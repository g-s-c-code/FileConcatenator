using System.Text;
using Spectre.Console;

namespace FileConcatenator;

public class Controller
{
	private readonly SpectreUI _ui;
	private readonly ConfigurationService _configurationService;
	private readonly ConcatenationService _concatenationService;
	private string _currentDirectory;
	private static readonly IReadOnlySet<string> _fileTypeChoices = Constants.FileExtensions.SupportedFileTypes;
	private const int WarningClipboardLimit = 10_000_000;

	public Controller(SpectreUI ui, ConfigurationService configurationService, ConcatenationService concatenationService)
	{
		_ui = ui;
		_configurationService = configurationService;
		_concatenationService = concatenationService;
		_currentDirectory = _configurationService.BaseDirectoryPath;
	}

	public void Run()
	{
		while (true)
		{
			try
			{
				RenderUI();
				ProcessCommand(_ui.GetInput("Enter command:"));
			}
			catch (Exception exception)
			{
				_ui.ShowMessageAndWait($"Unexpected error: {exception.Message}");
			}
		}
	}

	private void ProcessCommand(string command)
	{
		var normalizedCommand = command.ToLower();
		if (normalizedCommand.StartsWith(Constants.Commands.ChangeDirectoryPrefix))
		{
			ChangeDirectory(command);
			return;
		}

		switch (normalizedCommand)
		{
			case Constants.Commands.ConcatenateAndCopy:
				ConcatenateFilesAndCopyToClipboard();
				break;
			case Constants.Commands.SetClipboardLimit:
				SetClipboardLimit();
				break;
			case Constants.Commands.SetFileTypes:
				SetFileTypes();
				break;
			case Constants.Commands.SetBasePathManual:
				SetBasePath();
				break;
			case Constants.Commands.SetBasePathCurrent:
				SetBasePathToCurrentDirectory();
				break;
			case Constants.Commands.ShowHiddenFiles:
				SetShowHiddenFiles();
				break;
			case Constants.Commands.Help:
				ShowHelp();
				break;
			case Constants.Commands.Quit:
				Environment.Exit(0);
				break;
			default:
				_ui.ShowMessageAndWait("Error: Invalid command.");
				break;
		}
	}

	private void RenderUI()
	{
		_ui.Clear();
		_ui.MainLayout(
			_currentDirectory,
			BuildCommandList(),
			BuildSettingsHeaders(),
			BuildCurrentSettings(),
			_concatenationService.GetDirectories(_currentDirectory),
			_concatenationService.GetFiles(_currentDirectory)
		);
	}

	private string BuildCommandList()
	{
		var commands = new[]
		{
			"[cd <directory>] Change Directory",
			"",
			"[1] Concatenate & Copy To Clipboard",
			"[2] Set Clipboard Limit",
			"[3] Set File Types",
			"[4] Set Base Path (enter manually)",
			"[5] Set Base Path to Current Directory",
			"[6] Show Hidden Files",
			"",
			"[H] Help",
			"[Q] Quit"
		};

		return string.Join(Environment.NewLine, commands.Select(Markup.Escape));
	}

	private string BuildSettingsHeaders() => string.Join(
		Environment.NewLine,
		"Clipboard Limit:",
		"Targeted File Types:",
		"Base Path:",
		"Show Hidden Files:"
	);

	private string BuildCurrentSettings() => string.Join(
		Environment.NewLine,
		_configurationService.ClipboardCharacterLimit.ToString(),
		_configurationService.FileTypes,
		_configurationService.BaseDirectoryPath,
		_configurationService.ShowHiddenFiles ? "Yes" : "No"
	);

	private void SetBasePathToCurrentDirectory()
	{
		_configurationService.SetBaseDirectoryPath(_currentDirectory);
		_ui.ShowMessageAndWait($"Base path updated to the current directory: {_currentDirectory}");
	}

	private void ShowHelp()
	{
		var helpSections = new Dictionary<string, string[]>
		{
			["Purpose"] = new[]
			{
				"Concatenate text files from a selected directory and copy the combined content to your clipboard."
			},
			["Commands"] = new[]
			{
				"[cd <directory>] - Change to the specified directory.",
				"[1] Concatenate & Copy - Combine files and copy to clipboard.",
				"[2] Set Clipboard Limit - Set max characters for clipboard.",
				"[3] Set File Types - Choose which file types to concatenate.",
				"[4] Set Base Path - Change base directory manually.",
				"[5] Set Base Path to Current Directory - Use current directory as base.",
				"[6] Show Hidden Files - Toggle visibility of hidden files.",
				"[H] Help - Show this help message.",
				"[Q] Quit - Exit the application."
			},
			["Tips"] = new[]
			{
				"- Use 'cd' to navigate to the desired folder before operations.",
				"- Set a reasonable clipboard limit to handle large text blocks.",
				"- Default file types are '*.cs' if none are selected.",
				"- Hidden files are not shown by default; toggle with [6].",
				"",
				"Note: Settings are persistent between sessions."
			}
		};

		var helpText = new StringBuilder("FILE CONCATENATOR\n\n");
		foreach (var section in helpSections)
		{
			helpText.AppendLine($"{section.Key}:")
				   .AppendLine(string.Join(Environment.NewLine, section.Value))
				   .AppendLine();
		}

		_ui.ShowMessageAndWait(helpText.ToString());
	}

	private void ChangeDirectory(string command)
	{
		var parts = command.Split(' ', 2);
		if (parts.Length != 2)
		{
			_ui.ShowMessageAndWait("Error: Invalid command.");
			return;
		}

		var newDirectory = Path.GetFullPath(Path.Combine(_currentDirectory, parts[1]));
		if (!Directory.Exists(newDirectory))
		{
			_ui.ShowMessageAndWait("Error: Directory does not exist.");
			return;
		}

		_currentDirectory = newDirectory;
	}

	private void ConcatenateFilesAndCopyToClipboard()
	{
		var result = _concatenationService.ConcatenateFiles(_currentDirectory);
		var message = result.Success
			? "Files concatenated and copied to clipboard."
			: $"Error: {result.Message}";
		_ui.ShowMessageAndWait(message);
	}

	private void SetShowHiddenFiles()
	{
		var showHiddenFiles = GetValidInput("Show hidden files? (y/n): ", new[] { "y", "n" });
		_configurationService.SetShowHiddenFiles(showHiddenFiles == "y");
		_ui.ShowMessageAndWait("Show hidden files setting updated.");
	}

	private void SetBasePath()
	{
		var newBasePath = _ui.GetInput("Enter new base path: ");
		if (!Directory.Exists(newBasePath))
		{
			_ui.ShowMessageAndWait("Error: Directory does not exist.");
			return;
		}

		_configurationService.SetBaseDirectoryPath(newBasePath);
		_currentDirectory = newBasePath;
		_ui.ShowMessageAndWait("Base path updated.");
	}

	private void SetFileTypes()
	{
		var fileTypes = PromptForFileTypes();
		if (fileTypes.Count == 0)
		{
			fileTypes.Add(Constants.FileExtensions.DefaultFileType);
			_ui.ShowMessageAndWait(
				$"No file types were selected, so '{Constants.FileExtensions.DefaultFileType}' was set as the default.\n");
		}

		_configurationService.SetFileTypes(string.Join(", ", fileTypes));
		_ui.ShowMessageAndWait("Targeted file types updated.");
	}

	private List<string> PromptForFileTypes()
	{
		var space = Markup.Escape("[space]");
		var enter = Markup.Escape("[enter]");

		return AnsiConsole.Prompt(
			new MultiSelectionPrompt<string>()
				.Title("\nSelect the file types you wish to concatenate:")
				.NotRequired()
				.PageSize(10)
				.MoreChoicesText("[white]Move up and down to reveal more file types[/]")
				.InstructionsText(
					$"[white]Press [steelblue1_1]{space}[/] to toggle a file type, [steelblue1_1]{enter}[/] to accept[/]")
				.AddChoices(_fileTypeChoices));
	}

	private void SetClipboardLimit()
	{
		_ui.ShowMessage(
			$"Warning: Setting a clipboard limit above {WarningClipboardLimit} characters might cause issues on some systems.\n");

		var input = _ui.GetInput(
			$"Enter new clipboard character limit (current: {_configurationService.ClipboardCharacterLimit}): ");

		if (!int.TryParse(input, out var newLimit) || newLimit <= 0)
		{
			_ui.ShowMessageAndWait("Error: Invalid clipboard limit. Please enter a positive integer.");
			return;
		}

		_configurationService.SetClipboardCharacterLimit(newLimit);
		var warningMessage = newLimit > WarningClipboardLimit
			? $"Warning: Setting a clipboard limit above {WarningClipboardLimit} characters might cause issues on some systems."
			: $"Clipboard character limit updated to {newLimit}.";
		_ui.ShowMessageAndWait(warningMessage);
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