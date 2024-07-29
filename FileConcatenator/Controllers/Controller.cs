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

			_ui.DisplayMessage($"Current Directory: {_currentDirectory}");
			_ui.DisplayMessage($"Current Targeted File Types: {targetedFileTypes}");
			_ui.DisplayMessage("");

			DisplayDirectories(_currentDirectory);
			_ui.DisplayMessage("");
			DisplayFiles(_currentDirectory);
			_ui.DisplayMessage("");

			_ui.DisplayMessage("Commands:");
			_ui.DisplayMessage("[cd <directory>] - Change Directory");
			_ui.DisplayMessage("[1] - Concatenate files and copy to clipboard");
			_ui.DisplayMessage("[2] - Configure Settings");
			_ui.DisplayMessage("[3] - Exit application");
			_ui.DisplayMessage("Enter command: ");
			string command = _ui.GetInput();

			if (command.StartsWith("cd"))
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
			else if (command == "1")
			{
				ConcatenateFilesAndCopyToClipboard(_currentDirectory);
				_ui.DisplayMessage("Press any key to continue.");
				Console.ReadKey();
			}
			else if (command == "2")
			{
				ConfigureSettings();
				_currentDirectory = _configService.GetBaseDirectoryPath(); // Update in case the base path was changed
			}
			else if (command == "3")
			{
				break;
			}
			else
			{
				_ui.DisplayMessage("Error: Invalid command.");
			}

		}
	}

	private void DisplayDirectories(string path)
	{
		var directories = _fileConcatenationService.GetDirectories(path);
		_ui.DisplayDirectories(directories);
	}

	private void DisplayFiles(string path)
	{
		var files = _fileConcatenationService.GetFiles(path);
		_ui.DisplayFiles(files);
	}

	private void ConcatenateFilesAndCopyToClipboard(string path)
	{
		var result = _fileConcatenationService.ConcatenateFiles(path);
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
			_ui.DisplayMessage("Configuration Settings:");
			_ui.DisplayMessage($"[1] Show hidden files - Current Settings: {_configService.GetShowHiddenFiles()}");
			_ui.DisplayMessage($"[2] Set Base Path - Current Settings: {_configService.GetBaseDirectoryPath()}");
			_ui.DisplayMessage($"[3] File types to concatenate - Current Settings: {_configService.GetTargetedFileTypes()}");
			_ui.DisplayMessage($"[4] Clipboard character limit - Current Settings: {_configService.GetClipboardCharacterLimit()}");
			_ui.DisplayMessage("[5] Back to main menu");
			_ui.DisplayMessage("");
			_ui.DisplayMessage("Enter the number of the setting you want to change: ");
			string choice = _ui.GetInput();
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
		_ui.DisplayMessage("Show hidden files? (yes/no): ");
		string showHiddenFiles = _ui.GetInput().ToLower();

		while (showHiddenFiles != "yes" && showHiddenFiles != "no")
		{
			_ui.DisplayMessage("Invalid input. Please enter 'yes' or 'no'.");
			showHiddenFiles = _ui.GetInput().ToLower();
		}

		_configService.SetShowHiddenFiles(showHiddenFiles == "yes");
	}

	private void ConfigureBasePath()
	{
		_ui.DisplayMessage("Enter new base path: ");
		string newBasePath = _ui.GetInput();
		if (Directory.Exists(newBasePath))
		{
			_configService.SetBaseDirectoryPath(newBasePath);
		}
		else
		{
			_ui.DisplayMessage("Error: Directory does not exist.");
		}
	}

	private void ConfigureFileTypes()
	{
		_ui.DisplayMessage("Enter the file types you wish to concatenate (comma separated, e.g., *.cs, *.js):");
		string fileTypes = _ui.GetInput();
		_configService.SetTargetedFileTypes(fileTypes);
	}

	private void ConfigureClipboardLimit()
	{
		_ui.DisplayMessage($"Enter new clipboard character limit (current: {_configService.GetClipboardCharacterLimit()}): ");
		if (int.TryParse(_ui.GetInput(), out int newLimit) && newLimit > 0)
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
