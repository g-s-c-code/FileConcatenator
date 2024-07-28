namespace FileConcatenator;

public class ConfigurationController
{
	private readonly IUserInterface _ui;
	private readonly ConfigurationService _configService;

	public ConfigurationController(IUserInterface ui, ConfigurationService configService)
	{
		_ui = ui;
		_configService = configService;
	}

	public void ConfigureSettings()
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
