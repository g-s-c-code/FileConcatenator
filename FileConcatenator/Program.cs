using FileConcatenator.Services;

namespace FileConcatenator;

internal class Program
{
	private static void Main(string[] args)
	{
		var configService = new ConfigurationService();
		var fileConcatenationService = new FileConcatenationService(configService.Config);

		string currentDirectory = configService.Config.BasePath;

		while (true)
		{
			Console.Clear();
			Console.WriteLine($"Current Directory: {currentDirectory}");
			fileConcatenationService.DisplayFileTypes();
			Console.WriteLine();

			Console.WriteLine("Directories:");
			fileConcatenationService.DisplayDirectories(currentDirectory);
			Console.WriteLine();

			Console.WriteLine("Files:");
			fileConcatenationService.DisplayFiles(currentDirectory);
			Console.WriteLine();

			Console.WriteLine("Commands:");
			Console.WriteLine("[cd <directory>] - Change Directory");
			Console.WriteLine("[1] - Concatenate files and copy to clipboard");
			Console.WriteLine("[2] - Configure Settings");
			Console.WriteLine("[3] - Exit application");
			Console.WriteLine();

			Console.Write("Enter command: ");
			string command = Console.ReadLine();
			Console.WriteLine();

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
						Console.WriteLine();
					}
				}
			}
			else if (command == "1")
			{
				fileConcatenationService.ConcatenateFilesAndCopyToClipboard(currentDirectory);
				Console.WriteLine("Files concatenated and copied to clipboard.");
				Console.WriteLine();
			}
			else if (command == "2")
			{
				configService.ConfigureSettings();
				currentDirectory = configService.Config.BasePath;
			}
			else if (command == "3")
			{
				break;
			}
			else
			{
				Console.WriteLine("Error: Invalid command.");
				Console.WriteLine();
			}
			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
		}
	}
}