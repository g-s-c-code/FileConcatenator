using Spectre.Console;

namespace FileConcatenator;

public class Controller
{
	private readonly SpectreUI _ui;
	private readonly ConfigurationManager _configurationService;
	private readonly FileConcatenationService _fileConcatenationService;
	private string _currentDirectory;
	private bool _isInSettingsMenu;

	private const string no = "No";
	private const string yes = "Yes";

	public Controller(SpectreUI ui, ConfigurationManager configurationService, FileConcatenationService fileConcatenationService)
	{
		_ui = ui;
		_configurationService = configurationService;
		_currentDirectory = _configurationService.GetBaseDirectoryPath();
		_fileConcatenationService = fileConcatenationService;
		_isInSettingsMenu = false;
	}

	public void Run()
	{
		while (true)
		{
			_ui.Clear();

			var directoriesTree = _fileConcatenationService.GetDirectories(_currentDirectory);
			var filesTree = _fileConcatenationService.GetFiles(_currentDirectory);

			_ui.MainLayout(_configurationService.GetTargetedFileTypes(), _currentDirectory, DisplayAvailableCommands(), directoriesTree, filesTree);

			var userCommands = AnsiConsole.Ask<string>("Enter command:");

			ProcessCommand(userCommands);
		}
	}

	private string DisplayAvailableCommands()
	{
		if (_isInSettingsMenu)
		{
			return Markup.Escape(
				"[1] Show Hidden Files\n" +
				"[2] Set Base Path\n" +
				"[3] Set File Types\n" +
				"[4] Set Clipboard Limit\n" +
				"[5] Back\n" +
				"\n" +
				"Current Settings:\n" +
				$"Show Hidden Files: {(_configurationService.GetShowHiddenFiles() == false ? no : yes)}\n" +
				$"Base Path: {_configurationService.GetBaseDirectoryPath()}\n" +
				$"Targeted File Types: {_configurationService.GetTargetedFileTypes()}\n" +
				$"Clipboard Limit: {_configurationService.GetClipboardCharacterLimit()}"
			);
		}
		else
		{
			return Markup.Escape(
				"[cd <dir>] Change Directory\n" +
				"[1] Concatenate Files and Copy to Clipboard\n" +
				"[2] Configure Settings\n" +
				"[3] Exit"
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
			DisplayErrorMessage();
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
				DisplayErrorMessage();
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
				DisplayErrorMessage("Directory does not exist.");
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
		_configurationService.SetShowHiddenFiles(showHiddenFiles == "yes");
	}

	private void ConfigureBasePath()
	{
		string newBasePath = _ui.GetInput("Enter new base path: ");
		if (Directory.Exists(newBasePath))
		{
			_configurationService.SetBaseDirectoryPath(newBasePath);
			_currentDirectory = newBasePath;
		}
		else
		{
			_ui.DisplayMessage("Error: Directory does not exist.");
			_ui.GetInput("Press any key to continue...");
		}
	}

	private void ConfigureFileTypes()
	{
		string fileTypes = _ui.GetInput("Enter the file types you wish to concatenate (comma separated, e.g., *.cs, *.js): ");
		_configurationService.SetTargetedFileTypes(fileTypes);
	}

	private void ConfigureClipboardLimit()
	{
		string input = _ui.GetInput($"Enter new clipboard character limit (current: {_configurationService.GetClipboardCharacterLimit()}): ");
		if (int.TryParse(input, out int newLimit) && newLimit > 0)
		{
			_configurationService.SetClipboardCharacterLimit(newLimit);
			_ui.DisplayMessage($"Clipboard character limit updated to {newLimit}.");
			_ui.GetInput("Press any key to continue...");
		}
		else
		{
			_ui.DisplayMessage("Invalid input. Clipboard character limit remains unchanged.");
			_ui.GetInput("Press any key to continue...");
		}
	}

	public void DisplayErrorMessage(string errorMessage = "Invalid Command.")
	{
		_ui.DisplayMessage($"Error: {errorMessage}.");
		Console.ReadKey();
	}
}