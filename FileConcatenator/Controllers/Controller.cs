using Spectre.Console;

namespace FileConcatenator;

public class Controller
{
	private readonly SpectreUI _ui;
	private readonly ConfigurationService _configurationService;
	private readonly FileConcatenationService _fileConcatenationService;
	private string _currentDirectory;
	private static readonly string[] _fileTypeChoices =
	[
		"*.aspx", "*.bat", "*.c", "*.cc", "*.cfg", "*.cfm", "*.cgi", "*.class", "*.cmd",
		"*.com", "*.cpp", "*.cs", "*.css", "*.csv", "*.cxx", "*.dat", "*.db", "*.dbf", "*.env",
		"*.htm", "*.html", "*.ini", "*.java", "*.js", "*.json", "*.jsp", "*.jsx", "*.log",
		"*.m", "*.md", "*.php", "*.pl", "*.py", "*.rb", "*.sass", "*.scala", "*.scss",
		"*.sh", "*.sln", "*.sql", "*.swift", "*.tex", "*.ts", "*.vb", "*.vbs", "*.vcxproj",
		"*.xml", "*.yaml", "*.yml"
	];

	public Controller(SpectreUI ui, ConfigurationService configurationService, FileConcatenationService fileConcatenationService)
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
				RenderUI();
				var userCommand = _ui.GetInput("Enter command:");
				ProcessCommand(userCommand);
			}
			catch (Exception exception)
			{
				_ui.ShowMessageAndWait($"Unexpected error: {exception.Message}");
			}
		}
	}

	private void RenderUI()
	{
		_ui.Clear();
		_ui.MainLayout(
			_currentDirectory,
			GetCommands(),
			GetSettingsHeaders(),
			GetCurrentSettings(),
			_fileConcatenationService.GetDirectories(_currentDirectory),
			_fileConcatenationService.GetFiles(_currentDirectory)
		);
	}

	private string GetCommands()
	{
		var commands = new List<string>
		{
			"[1] Concatenate & Copy To Clipboard",
			"[2] Set Clipboard Limit",
			"[3] Set File Types",
			"[4] Set Base Path (enter manually)",
			"[5] Set Base Path to Current Directory",
			"[6] Show Hidden Files",
			"[7] Change Theme",
			"",
			"[cd <directory>] Change Directory",
			"",
			"[H] Help",
			"[Q] Quit"
		};

		return Markup.Escape(string.Join("\n", commands));
	}

	private string GetSettingsHeaders()
	{
		var settings = new List<string>
		{
			"Clipboard Limit:",
			"Targeted File Types:",
			"Base Path:",
			"Show Hidden Files:",
		};

		return string.Join("\n", settings);
	}

	private string GetCurrentSettings()
	{
		var settings = new List<string>
		{
			_configurationService.GetClipboardCharacterLimit().ToString(),
			_configurationService.GetTargetedFileTypes(),
			_configurationService.GetBaseDirectoryPath(),
			_configurationService.GetShowHiddenFiles() ? "Yes" : "No",
			""
		};

		return string.Join("\n", settings);
	}

	private void ProcessCommand(string command)
	{
		switch (command.ToLower())
		{
			case var changeDirectoryCommand when changeDirectoryCommand.StartsWith("cd"):
				ChangeDirectory(changeDirectoryCommand);
				break;
			case "1":
				ConcatenateFilesAndCopyToClipboard();
				break;
			case "2":
				ConfigureClipboardLimit();
				break;
			case "3":
				ConfigureFileTypes();
				break;
			case "4":
				ConfigureBasePath();
				break;
			case "5":
				SetBasePathToCurrentDirectory();
				break;
			case "6":
				ConfigureShowHiddenFiles();
				break;
			case "7":
				ChangeTheme();
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

	private void SetBasePathToCurrentDirectory()
	{
		_configurationService.SetBaseDirectoryPath(_currentDirectory);
		_ui.ShowMessageAndWait($"Base path updated to the current directory: {_currentDirectory}");
	}

	private void ShowHelp()
	{
		var helpText = new List<string>
		{
			"FILE CONCATENATOR",
			"",
			"Purpose:",
			"Concatenate text files from a selected directory and copy the combined content to your clipboard.",
			"",
			"Commands:",
			"[cd <directory>] - Change to the specified directory.",
			"[1] Concatenate & Copy - Combine files and copy to clipboard.",
			"[2] Set Clipboard Limit - Set max characters for clipboard.",
			"[3] Set File Types - Choose which file types to concatenate.",
			"[4] Set Base Path - Change base directory manually.",
			"[5] Set Base Path to Current Directory - Use current directory as base.",
			"[6] Show Hidden Files - Toggle visibility of hidden files.",
			"[h] Help - Show this help message.",
			"[q] Quit - Exit the application.",
			"",
			"Tips:",
			"- Use 'cd' to navigate to the desired folder before operations.",
			"- Set a reasonable clipboard limit to handle large text blocks.",
			"- Default file types are '*.cs' if none are selected.",
			"- Hidden files are not shown by default; toggle with [6].",
			"",
			"Note: Settings are persistent between sessions."
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
				.AddChoices(_fileTypeChoices));

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
				_ui.ShowMessage("Invalid command. Enter \"y\" or \"n\".\n");
			}
		} while (!validOptions.Contains(input));
		return input;
	}

	private void ChangeTheme()
	{
		var choice = AnsiConsole.Prompt(
			new SelectionPrompt<string>()
				.Title("Select a theme:")
				.AddChoices(Program.Themes.Keys));

		_ui.SetTheme(Program.Themes[choice]);
		_configurationService.SetSelectedTheme(choice);
		_ui.ShowMessageAndWait("Theme updated.");
	}

	private bool DirectoryExists(string path) => Directory.Exists(path);
}
