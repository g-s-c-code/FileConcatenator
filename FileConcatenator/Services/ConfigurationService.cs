using Newtonsoft.Json;

public class ConfigurationService
{
	private readonly string _settingsFilePath;
	private Configuration _configuration;

	public ConfigurationService()
	{
		_settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
		_configuration = LoadOrCreateConfiguration();
	}

	public bool ShowHiddenFiles => _configuration.ShowHiddenFiles;
	public int ClipboardCharacterLimit => _configuration.ClipboardCharacterLimit;
	public string BaseDirectoryPath => _configuration.BaseDirectoryPath ?? "/";
	public string FileTypes => _configuration.FileTypes ?? string.Empty;

	private Configuration LoadOrCreateConfiguration()
	{
		var config = TryLoadConfiguration() ?? new Configuration();

		if (string.IsNullOrEmpty(config.BaseDirectoryPath))
		{
			config.BaseDirectoryPath = GetInitialBaseDirectoryPath();
		}

		SaveConfiguration(config);
		return config;
	}

	private Configuration? TryLoadConfiguration()
	{
		if (!File.Exists(_settingsFilePath))
		{
			return null;
		}

		try
		{
			return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(_settingsFilePath));
		}
		catch (Exception)
		{
			return null;
		}
	}

	private void SaveConfiguration(Configuration configuration)
	{
		File.WriteAllText(_settingsFilePath, JsonConvert.SerializeObject(configuration, Formatting.Indented));
	}

	private static string GetInitialBaseDirectoryPath()
	{
		if (Environment.OSVersion.Platform != PlatformID.Win32NT)
		{
			return "/";
		}

		var drives = DriveInfo.GetDrives();
		return drives.Length > 0 ? drives[0].RootDirectory.FullName : "/";
	}

	public void SetShowHiddenFiles(bool value)
	{
		_configuration.ShowHiddenFiles = value;
		SaveConfiguration(_configuration);
	}

	public void SetClipboardCharacterLimit(int value)
	{
		_configuration.ClipboardCharacterLimit = value;
		SaveConfiguration(_configuration);
	}

	public void SetBaseDirectoryPath(string path)
	{
		_configuration.BaseDirectoryPath = path;
		SaveConfiguration(_configuration);
	}

	public void SetFileTypes(string types)
	{
		_configuration.FileTypes = NormalizeFileTypes(types);
		SaveConfiguration(_configuration);
	}

	private static string NormalizeFileTypes(string types)
	{
		if (string.IsNullOrWhiteSpace(types))
		{
			return string.Empty;
		}

		return string.Join(", ", types
			.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
			.Select(s => s.Trim()));
	}
}