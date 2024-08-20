using System.Text;
using Spectre.Console;

namespace FileConcatenator;

public class Controller
{
	private readonly SpectreUI _ui;
	private readonly ConfigurationService _configurationService;
	private readonly ConcatenationService _concatenationService;
	private string _currentDirectory;
	private static readonly string[] _fileTypeChoices = Constants.FileExtensions.DefaultFileTypes;

	public Controller(SpectreUI ui, ConfigurationService configurationService, ConcatenationService concatenationService)
	{
		_ui = ui;
		_configurationService = configurationService;
		_concatenationService = concatenationService;
		_currentDirectory = _configurationService.GetBaseDirectoryPath();
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
		switch (command.ToLower())
		{
			case var changeDirectoryCommand when changeDirectoryCommand.StartsWith(Constants.Commands.ChangeDirectoryPrefix):
				ChangeDirectory(changeDirectoryCommand);
				break;
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
			case Constants.Commands.ChangeTheme:
				ChangeTheme();
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
			GetCommands(),
			GetSettingsHeaders(),
			GetCurrentSettings(),
			_concatenationService.GetDirectories(_currentDirectory),
			_concatenationService.GetFiles(_currentDirectory)
		);
	}

	private string GetCommands()
	{
		var sb = new StringBuilder()
			.AppendLine(Markup.Escape("[cd <directory>] Change Directory"))
			.AppendLine()
			.AppendLine(Markup.Escape("[1] Concatenate & Copy To Clipboard"))
			.AppendLine(Markup.Escape("[2] Set Clipboard Limit"))
			.AppendLine(Markup.Escape("[3] Set File Types"))
			.AppendLine(Markup.Escape("[4] Set Base Path (enter manually)"))
			.AppendLine(Markup.Escape("[5] Set Base Path to Current Directory"))
			.AppendLine(Markup.Escape("[6] Show Hidden Files"))
			.AppendLine(Markup.Escape("[7] Change Theme"))
			.AppendLine()
			.AppendLine(Markup.Escape("[H] Help"))
			.AppendLine(Markup.Escape("[Q] Quit"));

		return sb.ToString();
	}

	private string GetSettingsHeaders()
	{
		var sb = new StringBuilder()
			.AppendLine("Clipboard Limit:")
			.AppendLine("Targeted File Types:")
			.AppendLine("Base Path:")
			.Append("Show Hidden Files:");

		return sb.ToString();
	}

	private string GetCurrentSettings()
	{
		var sb = new StringBuilder()
			.AppendLine(_configurationService.GetClipboardCharacterLimit().ToString())
			.AppendLine(_configurationService.GetTargetedFileTypes())
			.AppendLine(_configurationService.GetBaseDirectoryPath())
			.AppendLine(_configurationService.GetShowHiddenFiles() ? "Yes" : "No");

		return sb.ToString();
	}

	private void SetBasePathToCurrentDirectory()
	{
		_configurationService.SetBaseDirectoryPath(_currentDirectory);
		_ui.ShowMessageAndWait($"Base path updated to the current directory: {_currentDirectory}");
	}

	private void ShowHelp()
	{
		var sb = new StringBuilder()
			.AppendLine("FILE CONCATENATOR")
			.AppendLine()
			.AppendLine("Purpose:")
			.AppendLine("Concatenate text files from a selected directory and copy the combined content to your clipboard.")
			.AppendLine()
			.AppendLine("Commands:")
			.AppendLine("[cd <directory>] - Change to the specified directory.")
			.AppendLine("[1] Concatenate & Copy - Combine files and copy to clipboard.")
			.AppendLine("[2] Set Clipboard Limit - Set max characters for clipboard.")
			.AppendLine("[3] Set File Types - Choose which file types to concatenate.")
			.AppendLine("[4] Set Base Path - Change base directory manually.")
			.AppendLine("[5] Set Base Path to Current Directory - Use current directory as base.")
			.AppendLine("[6] Show Hidden Files - Toggle visibility of hidden files.")
			.AppendLine("[H] Help - Show this help message.")
			.AppendLine("[Q] Quit - Exit the application.")
			.AppendLine()
			.AppendLine("Tips:")
			.AppendLine("- Use 'cd' to navigate to the desired folder before operations.")
			.AppendLine("- Set a reasonable clipboard limit to handle large text blocks.")
			.AppendLine("- Default file types are '*.cs' if none are selected.")
			.AppendLine("- Hidden files are not shown by default; toggle with [6].")
			.AppendLine()
			.Append("Note: Settings are persistent between sessions.");

		_ui.ShowMessageAndWait(sb.ToString());
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
		var result = _concatenationService.ConcatenateFiles(_currentDirectory);
		var message = result.Success ? "Files concatenated and copied to clipboard." : $"Error: {result.Message}";
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

	private void SetFileTypes()
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
			fileTypes.Add(Constants.DefaultFileType);
			_ui.ShowMessageAndWait($"No file types were selected, so '{Constants.DefaultFileType}' was set as the default.\n");
		}

		_configurationService.SetTargetedFileTypes(string.Join(", ", fileTypes));
		_ui.ShowMessageAndWait("Targeted file types updated.");
	}

	private void SetClipboardLimit()
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
}
