using Spectre.Console;

namespace FileConcatenator;

public class Controller
{
	private readonly SpectreUI _ui;
	private readonly ConfigurationManager _configService;
	private readonly FileConcatenationService _fileConcatenationService;
	private string _currentDirectory;

	public Controller(SpectreUI ui, ConfigurationManager configService, FileConcatenationService fileConcatenationService)
	{
		_ui = ui;
		_configService = configService;
		_fileConcatenationService = fileConcatenationService;
		_currentDirectory = _configService.GetBaseDirectoryPath();
	}

	public void Run()
	{
		while (true)
		{
			string targetedFileTypes = _configService.GetTargetedFileTypes();

			_ui.Clear();

			var currentDirectoryPanel = _ui.DisplayMarkup(_currentDirectory);
			var targetedFilesPanel = _ui.DisplayMarkup(targetedFileTypes);
			var commandsPanelContent = GetCommandList();
			var directoriesTree = _ui.DisplayTree("Directories", _fileConcatenationService.GetDirectories(_currentDirectory));
			var filesTree = _ui.DisplayTree("Files", _fileConcatenationService.GetFiles(_currentDirectory));

			_ui.MainLayout(targetedFilesPanel, currentDirectoryPanel, _ui.DisplayMarkup(commandsPanelContent), directoriesTree, filesTree);

			var userCommands = AnsiConsole.Ask<string>("[bold]Enter command:[/]");

			ProcessCommand(userCommands);
		}
	}

	private string GetCommandList()
	{
		var commands = "Commands:\n" +
			"cd <directory> - Change Directory\n" +
					   "1 - Concatenate files and copy to clipboard\n" +
					   "2 - Configure Settings\n" +
					   "3 - Exit application";

		return Markup.Escape(commands);
	}

	private void ProcessCommand(string command)
	{
		if (command.StartsWith("cd"))
		{
			ChangeDirectory(command);
		}
		else if (command == "1")
		{
			ConcatenateFilesAndCopyToClipboard();
		}
		else if (command == "2")
		{
			ConfigureSettings();
		}
		else if (command == "3")
		{
			Environment.Exit(0);
		}
		else
		{
			_ui.DisplayMessage("Error: Invalid command.");
		}
	}

	private void ChangeDirectory(string command)
	{
		string[] parts = command.Split(' ', 2);
		if (parts.Length == 2)
		{
			string newDirectory = Path.GetFullPath(Path.Combine(_currentDirectory, parts[1]));
			if (Directory.Exists(newDirectory))
			{
				_currentDirectory = newDirectory;
			}
			else
			{
				_ui.DisplayMessage("Error: Directory does not exist.");
			}
		}
	}

	private void ConcatenateFilesAndCopyToClipboard()
	{
		var result = _fileConcatenationService.ConcatenateFiles(_currentDirectory);
		_ui.DisplayMessage(result.Message);
		if (result.Success)
		{
			_ui.DisplayMessage("Files concatenated and copied to clipboard.");
		}
		else
		{
			_ui.DisplayMessage("Error occurred during concatenation.");
		}
	}

	private void ConfigureSettings()
	{
		while (true)
		{
			_ui.Clear();
			_ui.DisplayMessage("Configuration Settings:");
			_ui.DisplayMessage($"1 - Show hidden files - Current Settings: {_configService.GetShowHiddenFiles()}");
			_ui.DisplayMessage($"2 - Set Base Path - Current Settings: {_configService.GetBaseDirectoryPath()}");
			_ui.DisplayMessage($"3 - File types to concatenate - Current Settings: {_configService.GetTargetedFileTypes()}");
			_ui.DisplayMessage($"4 - Clipboard character limit - Current Settings: {_configService.GetClipboardCharacterLimit()}");
			_ui.DisplayMessage($"5 - Back to main menu");
			_ui.DisplayMessage("");

			string choice = _ui.GetInput("Enter the number of the setting you want to change: ");
			_ui.DisplayMessage("");

			switch (choice)
			{
				case "1":
					ConfigureShowHiddenFiles();
					break;
				case "2":
					ConfigureBasePath();
					break;
				case "3":
					ConfigureFileTypes();
					break;
				case "4":
					ConfigureClipboardLimit();
					break;
				case "5":
					return;
				default:
					_ui.DisplayMessage("Invalid choice.");
					break;
			}
		}
	}

	private void ConfigureShowHiddenFiles()
	{
		string showHiddenFiles = _ui.GetInput("Show hidden files? (yes/no): ").ToLower();
		while (showHiddenFiles != "yes" && showHiddenFiles != "no")
		{
			_ui.DisplayMessage("Invalid input. Please enter 'yes' or 'no'.");
			showHiddenFiles = _ui.GetInput("Show hidden files? (yes/no): ").ToLower();
		}
		_configService.SetShowHiddenFiles(showHiddenFiles == "yes");
	}

	private void ConfigureBasePath()
	{
		string newBasePath = _ui.GetInput("Enter new base path: ");
		if (Directory.Exists(newBasePath))
		{
			_configService.SetBaseDirectoryPath(newBasePath);
			_currentDirectory = newBasePath;
		}
		else
		{
			_ui.DisplayMessage("Error: Directory does not exist.");
		}
	}

	private void ConfigureFileTypes()
	{
		string fileTypes = _ui.GetInput("Enter the file types you wish to concatenate (comma separated, e.g., *.cs, *.js): ");
		_configService.SetTargetedFileTypes(fileTypes);
	}

	private void ConfigureClipboardLimit()
	{
		string input = _ui.GetInput($"Enter new clipboard character limit (current: {_configService.GetClipboardCharacterLimit()}): ");
		if (int.TryParse(input, out int newLimit) && newLimit > 0)
		{
			_configService.SetClipboardCharacterLimit(newLimit);
			_ui.DisplayMessage($"Clipboard character limit updated to {newLimit}.");
		}
		else
		{
			_ui.DisplayMessage("Invalid input. Clipboard character limit remains unchanged.");
		}
	}
}
