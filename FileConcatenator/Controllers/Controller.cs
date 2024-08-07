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
			_currentDirectory,
			GetCommands(),
			GetSettings(),
			directoriesTree,
			filesTree
		);
	}

	private string GetCommands() => GetMainMenu();

	private string GetMainMenu()
	{
		var title = _ui.StyledText("Commands:", Color.Grey78).ToUpper();

		var commands = new List<string>
		{
			"[cd <dir>] Change Directory",
			"[1] Concatenate & Copy Clipboard",
			"[2] Set Base Path",
			"[3] Set Clipboard Limit",
			"[4] Set File Types",
			"[5] Show Hidden Files",
			"",
			"[H] Help",
			"[Q] Quit"
		};

		return $"{title}\n" + Markup.Escape(string.Join("\n", commands));
	}

	private string GetSettings()
	{
		var settings = new List<string>
		{
			$"Base Path: {_ui.StyledText(_configurationService.GetBaseDirectoryPath(), Color.SteelBlue1_1)}",
			$"Clipboard Limit: {_ui.StyledText(_configurationService.GetClipboardCharacterLimit().ToString(), Color.SteelBlue1_1)}",
			$"Show Hidden Files: {_ui.StyledText(_configurationService.GetShowHiddenFiles() ? "Yes" : "No", Color.SteelBlue1_1)}",
			$"Targeted File Types: {_ui.StyledText(_configurationService.GetTargetedFileTypes(), Color.SteelBlue1_1)}",
			""
		};

		return string.Join("\n", settings);
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
				ConfigureBasePath();
				break;
			case "3":
				ConfigureClipboardLimit();
				break;
			case "4":
				ConfigureFileTypes();
				break;
			case "5":
				ConfigureShowHiddenFiles();
				break;
			case "h":
				ShowHelp();
				break;
			case "q":
				Environment.Exit(0);
				break;
			default:
				_ui.ShowMessageAndWait("Error: Invalid command.");
				break;
		}
	}

	private void ShowHelp()
	{
		var helpText = new List<string>
	{
		"File Concatenator Application",
		"",
		"[bold underline]Purpose:[/]",
		"This application concatenates text files of specified types from a selected directory and copies the combined content to your clipboard.",
		"",
		"[bold underline]Usage Instructions:[/]",
		"[cd <directory>] - Changes the current directory to the specified path.",
		"[1] Concatenate & Copy Clipboard - Concatenates targeted files in the current directory and copies the result to the clipboard.",
		"[2] Set Base Path - Changes the base directory for file operations.",
		"[3] Set Clipboard Limit - Sets the maximum number of characters that can be copied to the clipboard.",
		"[4] Set File Types - Select the file types to concatenate (e.g., *.txt, *.cs, etc.).",
		"[5] Show Hidden Files - Toggles the visibility of hidden files in the directory listing.",
		"[h] Help - Displays this help information.",
		"[q] Quit - Exits the application.",
		"",
		"[bold underline]Tips & Recommendations:[/]",
		"- Use '[cd <directory>]' to navigate to the desired folder before running any operations.",
		"- Set a reasonable clipboard limit to avoid issues with large text blocks.",
		"- If no file types are selected, '*.html' will be used by default.",
		"- Hidden files are not shown by default; use the '[5] Show Hidden Files' option to include them.",
		"",
		"[bold]Remember:[/] All settings are persistent and will be saved between sessions, ensuring a seamless experience each time you run the application."
	};

		_ui.ShowMessageAndWait(string.Join("\n", helpText));
	}


	private void ChangeDirectory(string command)
	{
		var parts = command.Split(' ', 2);
		if (parts.Length == 2)
		{
			var newDirectory = Path.GetFullPath(Path.Combine(_currentDirectory, parts[1]));
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
		var message = result.Success ? "Files concatenated and copied to clipboard." : $"Error: {result.Message}";
		_ui.ShowMessageAndWait(message);
	}

	private void ConfigureShowHiddenFiles()
	{
		var showHiddenFiles = GetValidInput("Show hidden files? (y/n): ", new[] { "y", "n" });
		_configurationService.SetShowHiddenFiles(showHiddenFiles == "y");
		_ui.ShowMessageAndWait("Show hidden files setting updated.");
	}

	private void ConfigureBasePath()
	{
		var newBasePath = _ui.GetInput("Enter new base path: ");
		if (DirectoryExists(newBasePath))
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
				.Title("\nSelect the file types you wish to concatenate:")
				.NotRequired()
				.PageSize(10)
				.MoreChoicesText("[white]Move up and down to reveal more file types[/]")
				.InstructionsText($"[white]Press [steelblue1_1]{space}[/] to toggle a file type, [steelblue1_1]{enter}[/] to accept[/]")
				.AddChoices(new[]
				{
				"*.txt", "*.cs", "*.js", "*.html", "*.xml", "*.json", "*.css", "*.md",
				"*.py", "*.java", "*.cpp", "*.c", "*.h", "*.ts", "*.yaml", "*.yml"
				}));

		if (fileTypes.Count == 0)
		{
			fileTypes.Add("*.cs");
			_ui.ShowMessageAndWait("No file types were selected, so '*.cs' was set as the default.\n");
		}

		_configurationService.SetTargetedFileTypes(string.Join(", ", fileTypes));
		_ui.ShowMessageAndWait("Targeted file types updated.");
	}


	private void ConfigureClipboardLimit()
	{
		const int warningLimit = 10_000_000;
		_ui.ShowMessage($"Warning: Setting a clipboard limit above {warningLimit} characters might cause issues on some systems.\n");

		var input = _ui.GetInput($"Enter new clipboard character limit (current: {_configurationService.GetClipboardCharacterLimit()}): ");
		if (int.TryParse(input, out var newLimit) && newLimit > 0)
		{
			_configurationService.SetClipboardCharacterLimit(newLimit);
			var warningMessage = newLimit > warningLimit
				? $"Warning: Setting a clipboard limit above {warningLimit} characters might cause issues on some systems."
				: $"Clipboard character limit updated to {newLimit}.";
			_ui.ShowMessageAndWait(warningMessage);
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
				_ui.ShowMessage("Invalid command. Enter \"y\" or \"n\".");
			}
		} while (!validOptions.Contains(input));
		return input;
	}

	private bool DirectoryExists(string path) => Directory.Exists(path);
}
