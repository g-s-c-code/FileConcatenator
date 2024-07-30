using Spectre.Console;

namespace FileConcatenator;

public class Controller
{
	private readonly SpectreUI _ui;
	private readonly ConfigurationManager _configService;
	private readonly FileConcatenationService _fileConcatenationService;
	private string _currentDirectory;
	private bool _isInSettingsMenu;

	public Controller(SpectreUI ui, ConfigurationManager configService, FileConcatenationService fileConcatenationService)
	{
		_ui = ui;
		_configService = configService;
		_fileConcatenationService = fileConcatenationService;
		_currentDirectory = _configService.GetBaseDirectoryPath();
		_isInSettingsMenu = false;
	}

	public void Run()
	{
		while (true)
		{
			string targetedFileTypes = _configService.GetTargetedFileTypes();

			_ui.Clear();

			var currentDirectory = _ui.DisplayMarkup(_currentDirectory);
			var targetedFiles = _ui.DisplayMarkup(targetedFileTypes);
			var commands = GetCommandList();
			var directoriesTree = _ui.DisplayTree("Directories", _fileConcatenationService.GetDirectories(_currentDirectory));
			var filesTree = _ui.DisplayTree("Files", _fileConcatenationService.GetFiles(_currentDirectory));

			_ui.MainLayout(targetedFiles, currentDirectory, _ui.DisplayMarkup(commands), directoriesTree, filesTree);

			var userCommands = AnsiConsole.Ask<string>("[bold]Enter command:[/]");

			ProcessCommand(userCommands);
		}
	}

	private string GetCommandList()
	{
		if (_isInSettingsMenu)
		{
			return Markup.Escape(
				"1 - Show hidden files\n" +
				"2 - Set Base Path\n" +
				"3 - File types to concatenate\n" +
				"4 - Clipboard character limit\n" +
				"5 - Back to main menu"
			);
		}
		else
		{
			return Markup.Escape(
				"cd <directory> - Change Directory\n" +
				"1 - Concatenate files and copy to clipboard\n" +
				"2 - Configure Settings\n" +
				"3 - Exit application"
			);
		}
	}

	private void ProcessCommand(string command)
	{
		if (_isInSettingsMenu)
		{
			ProcessSettingsCommand(command);
		}
		else
		{
			ProcessMainCommand(command);
		}
	}

	private void ProcessMainCommand(string command)
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
			_isInSettingsMenu = true;
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

	private void ProcessSettingsCommand(string command)
	{
		switch (command)
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
				_isInSettingsMenu = false;
				break;
			default:
				_ui.DisplayMessage("Invalid choice.");
				break;
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