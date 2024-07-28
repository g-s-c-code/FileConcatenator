using FileConcatenator.Services;

namespace FileConcatenator;

internal class Program
{
	private static void Main(string[] args)
	{
		var configService = new ConfigurationService();
		var fileConcatenationService = new FileConcatenationService(configService.Config);
		IUserInterface ui = new ConsoleUI();

		string currentDirectory = configService.Config.BasePath;

		while (true)
		{
			ui.Clear();
			ui.DisplayMessage($"Current Directory: {currentDirectory}");
			ui.DisplayMessage($"Current Targeted File Types: {fileConcatenationService.GetTargetedFileTypes()}");
			ui.DisplayMessage("Directories:");
			fileConcatenationService.DisplayDirectories(currentDirectory);
			ui.DisplayMessage("Files:");
			fileConcatenationService.DisplayFiles(currentDirectory);
			ui.DisplayMessage("Commands:");
			ui.DisplayMessage("[cd <directory>] - Change Directory");
			ui.DisplayMessage("[1] - Concatenate files and copy to clipboard");
			ui.DisplayMessage("[2] - Configure Settings");
			ui.DisplayMessage("[3] - Exit application");
			ui.DisplayMessage("Enter command: ");
			string command = ui.GetInput();

			if (command.StartsWith("cd"))
			{
				string[] parts = command.Split(' ', 2);
				if (parts.Length == 2)
				{
					string newDirectory = Path.GetFullPath(Path.Combine(currentDirectory, parts[1]));
					if (Directory.Exists(newDirectory))
					{
						currentDirectory = newDirectory;
					}
					else
					{
						ui.DisplayMessage("Error: Directory does not exist.");
					}
				}
			}
			else if (command == "1")
			{
				fileConcatenationService.ConcatenateFilesAndCopyToClipboard(currentDirectory);
				//Do not simply run the following - conditional error message if length is exceeded
				ui.DisplayMessage("Files concatenated and copied to clipboard.");
				ui.DisplayMessage("Press any key to continue.");
				ui.GetInput();
			}
			else if (command == "2")
			{
				configService.ConfigureSettings();
				currentDirectory = configService.Config.BasePath;
				ui.GetInput();
			}
			else if (command == "3")
			{
				break;
			}
			else
			{
				ui.DisplayMessage("Error: Invalid command.");
			}
		}
	}
}