using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using TextCopy;

namespace FileConcatenator
{
	class Program
	{
		static string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
		static Config config;
		static bool isNewConfig = false;

		static void Main(string[] args)
		{
			LoadOrCreateConfig();

			string currentDirectory = config.BasePath;

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
					ConcatenateFilesAndCopyToClipboard(currentDirectory);
					Console.WriteLine("Files concatenated and copied to clipboard.");
					Console.WriteLine();
				}
				else if (command == "2")
				{
					ConfigureSettings();
					currentDirectory = config.BasePath; // Update current directory after configuration
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

		static void LoadOrCreateConfig()
		{
			if (File.Exists(configFilePath))
			{
				try
				{
					config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFilePath));
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error reading config file: {ex.Message}");
					config = CreateDefaultConfig();
					SaveConfig();
					isNewConfig = true;
				}
			}
			else
			{
				config = CreateDefaultConfig();
				isNewConfig = true;
				ConfigureSettings(); // Run configuration settings if config is newly created
				SaveConfig();
			}
		}

		static Config CreateDefaultConfig()
		{
			Console.WriteLine("Creating default configuration...");
			var newConfig = new Config
			{
				BasePath = GetInitialBasePath(),
				ShowHiddenFiles = false,
				FileTypes = new string[] { "*.cs" }
			};
			return newConfig;
		}

		static void SaveConfig()
		{
			File.WriteAllText(configFilePath, JsonConvert.SerializeObject(config, Formatting.Indented));
			Console.WriteLine($"Configuration saved to: {configFilePath}");
		}

		static void DisplayDirectories(string path)
		{
			try
			{
				foreach (var dir in Directory.GetDirectories(path))
				{
					if (!config.ShowHiddenFiles && (new DirectoryInfo(dir).Attributes & FileAttributes.Hidden) != 0)
						continue;
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
					if (!config.ShowHiddenFiles && (new FileInfo(file).Attributes & FileAttributes.Hidden) != 0)
						continue;
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

			foreach (var fileType in config.FileTypes)
			{
				var files = Directory.GetFiles(path, fileType, SearchOption.AllDirectories);
				foreach (var file in files)
				{
					sb.AppendLine($"//{Path.GetFileName(file)}");
					sb.AppendLine(File.ReadAllText(file));
					sb.AppendLine();
				}
			}

			ClipboardService.SetText(sb.ToString());
		}

		static void ConfigureSettings()
		{
			Console.WriteLine("Configuration Settings:");
			Console.WriteLine("[1] Show hidden files: " + (config.ShowHiddenFiles ? "Yes" : "No"));
			Console.WriteLine("[2] Set Base Path: " + config.BasePath);
			Console.WriteLine("[3] File types to concatenate: " + string.Join(", ", config.FileTypes));
			Console.WriteLine("[4] Back to main menu");
			Console.WriteLine();

			Console.Write("Enter the number of the setting you want to change: ");
			string choice = Console.ReadLine();
			Console.WriteLine();

			switch (choice)
			{
				case "1":
					Console.Write("Show hidden files? (yes/no): ");
					string showHiddenFiles = Console.ReadLine().ToLower();
					config.ShowHiddenFiles = showHiddenFiles == "yes";
					SaveConfig();
					break;
				case "2":
					Console.Write("Enter new base path: ");
					string newBasePath = Console.ReadLine();
					if (Directory.Exists(newBasePath))
					{
						config.BasePath = newBasePath;
						SaveConfig();
					}
					else
					{
						Console.WriteLine("Error: Directory does not exist.");
					}
					break;
				case "3":
					ConfigureFileTypes();
					SaveConfig();
					break;
				case "4":
					break;
				default:
					Console.WriteLine("Invalid choice.");
					break;
			}
		}

		static void ConfigureFileTypes()
		{
			Console.WriteLine("File Types Configuration:");
			Console.WriteLine("[1] All file types");
			Console.WriteLine("[2] Common coding file types (.ts, .js, .cs, etc.)");
			Console.WriteLine("[3] Custom file types");
			Console.WriteLine();

			Console.Write("Enter the number of the option you want to choose: ");
			string choice = Console.ReadLine();
			Console.WriteLine();

			switch (choice)
			{
				case "1":
					Console.WriteLine("Warning: This may cause a high load on the system.");
					config.FileTypes = new string[] { "*.*" };
					break;
				case "2":
					config.FileTypes = new string[] { "*.ts", "*.js", "*.cs", "*.html", "*.css" };
					break;
				case "3":
					Console.Write("Enter the file types to concatenate (comma separated, e.g., *.cs,*.js): ");
					string customFileTypes = Console.ReadLine();
					config.FileTypes = customFileTypes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
					break;
				default:
					Console.WriteLine("Invalid choice. Keeping the old file types.");
					break;
			}
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

		class Config
		{
			public string BasePath { get; set; }
			public bool ShowHiddenFiles { get; set; }
			public string[] FileTypes { get; set; }
		}
	}
}
