using FileConcatenator;

public class ProgramController
{
	private readonly IUserInterface _ui;
	private readonly ConfigurationController _configurationController;
	private readonly ConfigurationService _configurationService;
	private readonly FileConcatenationController _fileConcatenationController;
	private readonly FileConcatenationService _fileConcatenationService;

	public ProgramController(IUserInterface ui, ConfigurationController configurationController, ConfigurationService configurationService, FileConcatenationController fileConcatenationController, FileConcatenationService fileConcatenationService)
	{
		_ui = ui;
		_configurationController = configurationController;
		_configurationService = configurationService;
		_fileConcatenationController = fileConcatenationController;
		_fileConcatenationService = fileConcatenationService;
	}

	public void Run()
	{
		while (true)
		{
			string currentDirectory = _configurationService.GetBaseDirectoryPath();
			string targetedFileTypes = _configurationService.GetTargetedFileTypes();

			_ui.Clear();
			_ui.DisplayMessage($"Current Directory: {currentDirectory}");
			_ui.DisplayMessage($"Current Targeted File Types: {targetedFileTypes}");
			_ui.DisplayMessage("Directories:");
			_fileConcatenationController.DisplayDirectories(currentDirectory);
			_ui.DisplayMessage("Files:");
			_fileConcatenationController.DisplayFiles(currentDirectory);
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
					string newDirectory = Path.GetFullPath(Path.Combine(currentDirectory, parts[1]));
					if (Directory.Exists(newDirectory))
					{
						_configurationService.SetBaseDirectoryPath(newDirectory);
					}
					else
					{
						_ui.DisplayMessage("Error: Directory does not exist.");
					}
				}
			}
			else if (command == "1")
			{
				_fileConcatenationController.ConcatenateFilesAndCopyToClipboard(currentDirectory);
				_ui.DisplayMessage("Press any key to continue.");
				Console.ReadKey();
			}
			else if (command == "2")
			{
				_configurationController.ConfigureSettings();
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
}
