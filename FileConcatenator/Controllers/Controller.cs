using Spectre.Console;
using System.IO;

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
			try
			{
				_ui.Clear();
				DisplayUI();
				var userCommand = AnsiConsole.Ask<string>(_ui.StyledText("Enter command:", Color.White));
				ProcessCommand(userCommand);
			}
			catch (Exception ex)
			{
				_ui.ShowMessageAndWait($"Unexpected error: {ex.Message}");
			}
		}
	}

	private void DisplayUI()
	{
		var directoriesTree = _fileConcatenationService.GetDirectories(_currentDirectory);
		var filesTree = _fileConcatenationService.GetFiles(_currentDirectory);

		_ui.MainLayout(
			_configurationService.GetTargetedFileTypes(),
			_currentDirectory,
			DisplayAvailableCommands(),
			directoriesTree,
			filesTree
		);
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
			"[5] Back\n")
		+ "\n"
		+ GetCurrentSettings();
	}

	private string GetCurrentSettings()
	{
		return _ui.StyledText("Current Settings:\n", Color.Grey78).ToUpper() +
			$"Show Hidden Files: {_ui.StyledText(_configurationService.GetShowHiddenFiles() ? "Yes" : "No", Color.SteelBlue1_1)}\n" +
			$"Base Path: {_ui.StyledText(_configurationService.GetBaseDirectoryPath(), Color.SteelBlue1_1)}\n" +
			$"Targeted File Types: {_ui.StyledText(_configurationService.GetTargetedFileTypes(), Color.SteelBlue1_1)}\n" +
			$"Clipboard Limit: {_ui.StyledText(_configurationService.GetClipboardCharacterLimit().ToString(), Color.SteelBlue1_1)}";
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
		switch (command.ToLower())
		{
			case var cmd when cmd.StartsWith("cd"):
				ChangeDirectory(cmd);
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
		switch (command.ToLower())
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
		else
		{
			_ui.ShowMessageAndWait("Error: Invalid directory command.");
		}
	}

	private void ConcatenateFilesAndCopyToClipboard()
	{
		var result = _fileConcatenationService.ConcatenateFiles(_currentDirectory);
		if (result.Success)
		{
			_ui.ShowMessageAndWait("Files concatenated and copied to clipboard.");
		}
		else
		{
			_ui.ShowMessageAndWait($"Error: {result.Message}");
		}
	}

	private void ConfigureShowHiddenFiles()
	{
		var showHiddenFiles = GetValidInput("Show hidden files? (y/n): ", new[] { "y", "n" });
		_configurationService.SetShowHiddenFiles(showHiddenFiles == "y");
		_ui.ShowMessageAndWait("Show hidden files setting updated.");
	}

	private void ConfigureBasePath()
	{
		string newBasePath = _ui.GetInput("Enter new base path: ");
		if (Directory.Exists(newBasePath))
		{
			_configurationService.SetBaseDirectoryPath(newBasePath);
			_currentDirectory = newBasePath;
			_ui.ShowMessageAndWait("Base path updated.");
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
		_ui.ShowMessageAndWait("Targeted file types updated.");
	}

	private void ConfigureClipboardLimit()
	{
		const int warningLimit = 10000000;
		_ui.ShowMessage($"Warning: Setting a clipboard limit above {warningLimit} characters might cause issues on some systems.\n");
		string input = _ui.GetInput($"Enter new clipboard character limit (current: {_configurationService.GetClipboardCharacterLimit()}): ");

		if (int.TryParse(input, out int newLimit) && newLimit > 0)<
		{
			_configurationService.SetClipboardCharacterLimit(newLimit);
			if (newLimit > warningLimit)
			{
				_ui.ShowMessageAndWait($"Warning: Setting a clipboard limit above {warningLimit} characters might cause issues on some systems.");
			}
			else
			{
				_ui.ShowMessageAndWait($"Clipboard character limit updated to {newLimit}.");
			}
		}
		else
		{
			_ui.ShowMessageAndWait("Error: Invalid clipboard limit. Please enter a positive integer.");
		}
	}

	private string GetValidInput(string prompt, string[] validOptions)
	{
		string input;
		do
		{
			input = _ui.GetInput(prompt).ToLower();
			if (!validOptions.Contains(input))
			{
				_ui.ShowMessage("Invalid input. Enter \"y\" or \"n\".\n");
			}
		} while (!validOptions.Contains(input));
		return input;
	}
}
