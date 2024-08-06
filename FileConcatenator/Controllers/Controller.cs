using Spectre.Console;
using System.Collections.Generic;
using System.IO;

namespace FileConcatenator;

public class Controller
{
	private readonly SpectreUI _ui;
	private readonly ConfigurationManager _configurationService;
	private readonly FileConcatenationService _fileConcatenationService;
	private string _currentDirectory;

	public Controller(SpectreUI ui, ConfigurationManager configurationService, FileConcatenationService fileConcatenationService)
	{
		_ui = ui;
		_configurationService = configurationService;
		_fileConcatenationService = fileConcatenationService;
		_currentDirectory = _configurationService.GetBaseDirectoryPath();
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
		return GetMainMenu();
	}

	private string GetMainMenu()
	{
		return Markup.Escape(
			"[cd <dir>] Change Directory\n" +
			"[1] Concatenate & Copy Clipboard\n" +
			"[2] Show Hidden Files\n" +
			"[3] Set Base Path\n" +
			"[4] Set File Types\n" +
			"[5] Set Clipboard Limit\n" +
			"[Q] Quit" +
			"\n" +
			"\n" +
			"\n"
			) +
			GetCurrentSettings();

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
		switch (command.ToLower())
		{
			case var cmd when cmd.StartsWith("cd"):
				ChangeDirectory(cmd);
				break;
			case "1":
				ConcatenateFilesAndCopyToClipboard();
				break;
			case "2":
				ConfigureShowHiddenFiles();
				break;
			case "3":
				ConfigureBasePath();
				break;
			case "4":
				ConfigureFileTypes();
				break;
			case "5":
				ConfigureClipboardLimit();
				break;
			case "Q" or "q":
				Environment.Exit(0);
				break;
			default:
				_ui.ShowMessageAndWait("Error: Invalid command.");
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
			_ui.ShowMessageAndWait("Error: Invalid command.");
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
		var space = Markup.Escape("[space]");
		var enter = Markup.Escape("[enter]");

		var fileTypes = AnsiConsole.Prompt(
			new MultiSelectionPrompt<string>()
				.Title("Select the file types you wish to concatenate:")
				.NotRequired()
				.PageSize(10)
				.MoreChoicesText("[grey](Move up and down to reveal more file types)[/]")
				.InstructionsText(
					"[grey](Press [steelblue1_1]" + space + "[/] to toggle a file type, [steelblue1_1]" + enter + "[/] to accept)[/]")
				.AddChoices(new[]
				{
				"*.txt", "*.cs", "*.js", "*.html", "*.xml", "*.json", "*.css", "*.md",
				"*.py", "*.java", "*.cpp", "*.c", "*.h", "*.ts", "*.yaml", "*.yml"
				}));

		// Safety net: If no file types are selected, default to *.html
		if (fileTypes.Count == 0)
		{
			fileTypes.Add("*.html");
			_ui.ShowMessageAndWait("No file types were selected, so '*.html' was set as the default.\n");
		}

		_configurationService.SetTargetedFileTypes(string.Join(", ", fileTypes));
		_ui.ShowMessageAndWait("Targeted file types updated.");
	}

	private void ConfigureClipboardLimit()
	{
		const int warningLimit = 10000000;
		_ui.ShowMessage($"Warning: Setting a clipboard limit above {warningLimit} characters might cause issues on some systems.\n");
		string input = _ui.GetInput($"Enter new clipboard character limit (current: {_configurationService.GetClipboardCharacterLimit()}): ");

		if (int.TryParse(input, out int newLimit) && newLimit > 0)
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
				_ui.ShowMessage("Invalid command. Enter \"y\" or \"n\".\n");
			}
		} while (!validOptions.Contains(input));
		return input;
	}
}
