using FileConcatenator.Models;
using Newtonsoft.Json;

namespace FileConcatenator.Services;

public class ConfigurationService
{
	private readonly string configFilePath;
	public Configuration Config { get; private set; }
	public bool IsNewConfig { get; private set; }

	public ConfigurationService()
	{
		configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
		LoadOrCreateConfig();
	}

	public void LoadOrCreateConfig()
	{
		if (File.Exists(configFilePath))
		{
			try
			{
				Config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(configFilePath));
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error reading config file: {ex.Message}");
				Config = new Configuration();
				SaveConfig();
				IsNewConfig = true;
			}
		}
		else
		{
			Config = new Configuration();
			IsNewConfig = true;
			SaveConfig();
		}
	}

	public void SaveConfig()
	{
		File.WriteAllText(configFilePath, JsonConvert.SerializeObject(Config, Formatting.Indented));
		Console.WriteLine($"Configuration saved to: {configFilePath}");
	}

	public void ConfigureSettings()
	{
		Console.WriteLine("Configuration Settings:");
		Console.WriteLine("[1] Show hidden files: " + (Config.ShowHiddenFiles ? "Yes" : "No"));
		Console.WriteLine("[2] Set Base Path: " + Config.BasePath);
		Console.WriteLine("[3] File types to concatenate: " + string.Join(", ", Config.FileTypes));
		Console.WriteLine($"[4] Clipboard character limit: {Config.ClipboardCharacterLimit}");
		Console.WriteLine("[5] Back to main menu");
		Console.WriteLine();

		Console.Write("Enter the number of the setting you want to change: ");
		string choice = Console.ReadLine();
		Console.WriteLine();

		switch (choice)
		{
			case "1":
				Console.Write("Show hidden files? (yes/no): ");
				string showHiddenFiles = Console.ReadLine().ToLower();
				Config.ShowHiddenFiles = showHiddenFiles == "yes";
				SaveConfig();
				break;
			case "2":
				Console.Write("Enter new base path: ");
				string newBasePath = Console.ReadLine();
				if (Directory.Exists(newBasePath))
				{
					Config.BasePath = newBasePath;
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
				ConfigureClipboardLimit();
				SaveConfig();
				break;
			case "5":
				break;
			default:
				Console.WriteLine("Invalid choice.");
				break;
		}
	}

	private void ConfigureFileTypes()
	{
		Console.WriteLine("File Types Configuration:");
		Console.WriteLine("[1] All file types");
		Console.WriteLine("[2] Common developer file types (.ts, .js, .cs, etc.)");
		Console.WriteLine("[3] Custom file types");
		Console.WriteLine();

		Console.Write("Enter the number of the option you want to choose: ");
		string choice = Console.ReadLine();
		Console.WriteLine();

		switch (choice)
		{
			case "1":
				Console.WriteLine("Warning: This may cause a high load on the system.");
				Config.FileTypes = new string[] { "*.*" };
				break;
			case "2":
				Config.FileTypes = new string[] { "*.ts", "*.js", "*.cs", "*.html", "*.css" };
				break;
			case "3":
				Console.Write("Enter the file types to concatenate (comma separated, e.g., *.cs,*.js): ");
				string customFileTypes = Console.ReadLine();
				Config.FileTypes = customFileTypes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
				break;
			default:
				Console.WriteLine("Invalid choice. Keeping the old file types.");
				break;
		}
	}

	private void ConfigureClipboardLimit()
	{
		Console.Write("Enter new clipboard character limit (current: " + Config.ClipboardCharacterLimit + "): ");
		if (int.TryParse(Console.ReadLine(), out int newLimit) && newLimit > 0)
		{
			Config.ClipboardCharacterLimit = newLimit;
			Console.WriteLine($"Clipboard character limit updated to {newLimit}.");
		}
		else
		{
			Console.WriteLine("Invalid input. Clipboard character limit remains unchanged.");
		}
	}
}