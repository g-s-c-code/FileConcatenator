namespace FileConcatenator;

internal class Program
{
	private static void Main(string[] args)
	{
		var configService = new ConfigurationService();
		var fileConcatenationService = new FileConcatenationService(configService.LoadOrCreateConfig());
		var fileConcatenationController = new FileConcatenationController(new ConsoleUI(), fileConcatenationService);
		var configurationController = new ConfigurationController(new ConsoleUI());

		string currentDirectory = configService.GetBaseDirectoryPath();

		while (true)
		{
			Console.Clear();
			Console.WriteLine($"Current Directory: {currentDirectory}");
			Console.WriteLine($"Current Targeted File Types: {configService.GetTargetedFileTypes()}");
			Console.WriteLine("Directories:");
			fileConcatenationController.DisplayDirectories(currentDirectory);
			Console.WriteLine("Files:");
			fileConcatenationController.DisplayFiles(currentDirectory);
			Console.WriteLine("Commands:");
			Console.WriteLine("[cd <directory>] - Change Directory");
			Console.WriteLine("[1] - Concatenate files and copy to clipboard");
			Console.WriteLine("[2] - Configure Settings");
			Console.WriteLine("[3] - Exit application");
			Console.Write("Enter command: ");
			string command = Console.ReadLine();

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
						Console.WriteLine("Error: Directory does not exist.");
					}
				}
			}
			else if (command == "1")
			{
				fileConcatenationController.ConcatenateFilesAndCopyToClipboard(currentDirectory);
				Console.WriteLine("Press any key to continue.");
				Console.ReadKey();
			}
			else if (command == "2")
			{
				configurationController.ConfigureSettings();
				currentDirectory = configService.GetBaseDirectoryPath();
			}
			else if (command == "3")
			{
				break;
			}
			else
			{
				Console.WriteLine("Error: Invalid command.");
			}
		}
	}
}
