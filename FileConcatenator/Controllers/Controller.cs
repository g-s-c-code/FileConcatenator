using Spectre.Console;

namespace FileConcatenator;

public class Controller
{
	private readonly SpectreUI _ui;
	private readonly ConfigurationManager _configurationService;
	private readonly FileConcatenationService _fileConcatenationService;
	private string _currentDirectory;
	private bool _isInSettingsMenu;

	public Controller(SpectreUI ui, ConfigurationManager configurationService, FileConcatenationService fileConcatenationService)
	{
		_ui = ui;
		_configurationService = configurationService;
		_fileConcatenationService = fileConcatenationService;
		_currentDirectory = _configurationService.GetBaseDirectoryPath();
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

			var userCommands = AnsiConsole.Ask<string>(_ui.StyledText("Enter command:", Color.White));

			ProcessCommand(userCommands);
		}
	}

	private string DisplayAvailableCommands()
	{
		return _isInSettingsMenu ? GetSettingsMenu() : GetMainMenu();
	}

	private string GetMainMenu()
	{
		return Markup.Escape(
			"[cd <dir>] Change Directory\n" +
			"[1] Concatenate & Copy Clipboard\n" +
			"[2] Configure Settings\n" +
			"[3] Exit"
		);
	}

	private string GetSettingsMenu()
	{
		return Markup.Escape(
			"[1] Show Hidden Files\n" +
			"[2] Set Base Path\n" +
			"[3] Set File Types\n" +
			"[4] Set Clipboard Limit\n" +
			"[5] Back\n" +
			"\n") + GetCurrentSettings();
	}

	private string GetCurrentSettings()
	{
		return _ui.StyledText("Current Settings:\n", Color.Grey78).ToUpper() +
			"Show Hidden Files: " + _ui.StyledText((_configurationService.GetShowHiddenFiles() ? "Yes" : "No"), Color.SteelBlue1_1) + "\n" +
			"Base Path: " + _ui.StyledText(_configurationService.GetBaseDirectoryPath(), Color.SteelBlue1_1) + "\n" +
			"Targeted File Types: " + _ui.StyledText(_configurationService.GetTargetedFileTypes(), Color.SteelBlue1_1) + "\n" +
			"Clipboard Limit: " + _ui.StyledText(_configurationService.GetClipboardCharacterLimit().ToString(), Color.SteelBlue1_1);
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
		switch (command)
		{
			case var s when s.StartsWith("cd"):
				ChangeDirectory(s);
				break;
			case "1":
				ConcatenateFilesAndCopyToClipboard();
				break;
			case "2":
				_isInSettingsMenu = true;
				break;
			case "3":
				Environment.Exit(0);
				break;
			default:
				_ui.ShowMessageAndWait("Error: Invalid Command.");
				break;
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
				_ui.ShowMessageAndWait("Error: Invalid Command.");
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
				_ui.ShowMessageAndWait("Error: Directory does not exist.");
			}
		}
	}

	private void ConcatenateFilesAndCopyToClipboard()
	{
		var result = _fileConcatenationService.ConcatenateFiles(_currentDirectory);
		_ui.ShowMessage(result.Message);
		if (result.Success)
		{
			_ui.ShowMessageAndWait("Files concatenated and copied to clipboard.");
		}
		else
		{
			_ui.ShowMessageAndWait("Error: Files not concatenated.");
		}
	}

	private void ConfigureShowHiddenFiles()
	{
		string showHiddenFiles;
		do
		{
			showHiddenFiles = _ui.GetInput("Show hidden files? (y/n): ").ToLower();
			if (showHiddenFiles != "y" && showHiddenFiles != "n")
			{
				_ui.ShowMessage("Invalid input. Please enter 'y' or 'n'.");
			}
		} while (showHiddenFiles != "y" && showHiddenFiles != "n");

		_configurationService.SetShowHiddenFiles(showHiddenFiles == "y");
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
			_ui.ShowMessageAndWait("Error: Directory does not exist.");
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
			_ui.ShowMessageAndWait($"Clipboard character limit updated to {newLimit}.");
		}
		else
		{
			_ui.ShowMessageAndWait("Error. Invalid command.");
		}
	}
}
