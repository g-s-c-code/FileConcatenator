using Newtonsoft.Json;

namespace FileConcatenator;

public class ConfigurationManager
{
	private readonly string _settingsFilePath;
	private Configuration _configuration;

	public ConfigurationManager()
	{
		_settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
		_configuration = LoadOrCreateConfig();
	}

	public Configuration LoadOrCreateConfig()
	{
		if (File.Exists(_settingsFilePath))
		{
			try
			{
				_configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(_settingsFilePath)) ?? new Configuration();
			}
			catch (Exception)
			{
				_configuration = new Configuration();
			}
		}
		else
		{
			_configuration = new Configuration();
		}

		// If BaseDirectoryPath is not set, initialize it
		if (string.IsNullOrEmpty(_configuration.BaseDirectoryPath))
		{
			_configuration.BaseDirectoryPath = GetInitialBaseDirectoryPath();
		}

		SaveConfiguration();
		return _configuration;
	}

	private void SaveConfiguration()
	{
		File.WriteAllText(_settingsFilePath, JsonConvert.SerializeObject(_configuration, Formatting.Indented));
	}

	public string GetSelectedTheme()
	{
		return _configuration.Theme ?? "Default";
	}

	public void SetSelectedTheme(string theme)
	{
		_configuration.Theme = theme;
		SaveConfiguration();
	}

	private string GetInitialBaseDirectoryPath()
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

	public bool GetShowHiddenFiles()
	{
		return _configuration.ShowHiddenFiles;
	}

	public void SetShowHiddenFiles(bool showHiddenFiles)
	{
		_configuration.ShowHiddenFiles = showHiddenFiles;
		SaveConfiguration();
	}

	public int GetClipboardCharacterLimit()
	{
		return _configuration.ClipboardCharacterLimit;
	}

	public void SetClipboardCharacterLimit(int clipboardCharacterLimit)
	{
		_configuration.ClipboardCharacterLimit = clipboardCharacterLimit;
		SaveConfiguration();
	}

	public string GetBaseDirectoryPath()
	{
		return _configuration.BaseDirectoryPath ?? "/";
	}

	public void SetBaseDirectoryPath(string baseDirectoryPath)
	{
		_configuration.BaseDirectoryPath = baseDirectoryPath;
		SaveConfiguration();
	}

	public string GetTargetedFileTypes()
	{
		return _configuration.FileTypes ?? string.Empty;
	}

	public void SetTargetedFileTypes(string fileTypes)
	{
		if (string.IsNullOrWhiteSpace(fileTypes))
		{
			_configuration.FileTypes = string.Empty;
		}
		else
		{
			_configuration.FileTypes = string.Join(",", fileTypes
				.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => s.Trim()));
		}

		SaveConfiguration();
	}
}
