using System.Text;
using Newtonsoft.Json;
using TextCopy;

namespace FileConcatenator
{
	class Program
	{
		static string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
		static string baseDirectory = LoadBasePath();

		static void Main(string[] args)
		{
			string currentDirectory = baseDirectory;

			while (true)
			{
				Console.Clear();
				Console.WriteLine($"Current Directory: {currentDirectory}");
				Console.WriteLine();

				Console.WriteLine("Directories:");
				DisplayDirectories(currentDirectory);
				Console.WriteLine();

				Console.WriteLine("Files:");
				DisplayFiles(currentDirectory);
				Console.WriteLine();

				Console.WriteLine("Commands:");
				Console.WriteLine("[cd <directory>] - Change Directory");
				Console.WriteLine("[1] - Concatenate .cs files and copy to clipboard");
				Console.WriteLine("[2] - Set Base Path");
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
					ConcatenateFilesAndCopyToClipboard(currentDirectory);
					Console.WriteLine("Files concatenated and copied to clipboard.");
					Console.WriteLine();
				}
				else if (command == "2")
				{
					Console.Write("Enter new base path: ");
					string newBasePath = Console.ReadLine();
					if (Directory.Exists(newBasePath))
					{
						baseDirectory = newBasePath;
						SaveBasePath(baseDirectory);
						currentDirectory = baseDirectory;
						Console.WriteLine("Base path set successfully.");
					}
					else
					{
						Console.WriteLine("Error: Directory does not exist.");
					}
					Console.WriteLine();
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

		static void DisplayDirectories(string path)
		{
			try
			{
				foreach (var dir in Directory.GetDirectories(path))
				{
					Console.WriteLine($"[D] {Path.GetFileName(dir)}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
				Console.WriteLine();
			}
		}

		static void DisplayFiles(string path)
		{
			try
			{
				foreach (var file in Directory.GetFiles(path))
				{
					Console.WriteLine($"[F] {Path.GetFileName(file)}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
				Console.WriteLine();
			}
		}

		static void ConcatenateFilesAndCopyToClipboard(string path)
		{
			var sb = new StringBuilder();
			var files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);

			foreach (var file in files)
			{
				sb.AppendLine(File.ReadAllText(file));
				sb.AppendLine();
			}

			ClipboardService.SetText(sb.ToString());
		}

		static string GetInitialBasePath()
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				var drives = DriveInfo.GetDrives();
				return drives.Length > 0 ? drives[0].RootDirectory.FullName : "/";
			}
			else
			{
				return "/";
			}
		}

		static string LoadBasePath()
		{
			Console.WriteLine($"Loading base path from: {configFilePath}");
			Console.WriteLine();

			if (File.Exists(configFilePath))
			{
				try
				{
					var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFilePath));
					if (config != null && !string.IsNullOrWhiteSpace(config.BasePath) && Directory.Exists(config.BasePath))
					{
						return config.BasePath;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error reading config file: {ex.Message}");
					Console.WriteLine();
				}
			}

			Console.WriteLine("Using default base path.");
			Console.WriteLine();
			return GetInitialBasePath();
		}

		static void SaveBasePath(string basePath)
		{
			var config = new Config { BasePath = basePath };
			File.WriteAllText(configFilePath, JsonConvert.SerializeObject(config, Formatting.Indented));
			Console.WriteLine($"Saved base path to: {configFilePath}");
			Console.WriteLine();
		}

		class Config
		{
			public string BasePath { get; set; }
		}
	}
}