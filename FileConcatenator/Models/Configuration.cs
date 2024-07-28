namespace FileConcatenator.Models;

public class Configuration
{
	public string BasePath { get; set; }
	public bool ShowHiddenFiles { get; set; }
	public string[] FileTypes { get; set; }
	public int ClipboardCharacterLimit { get; set; }

	public Configuration()
	{
		BasePath = GetInitialBasePath();
		ShowHiddenFiles = false;
		FileTypes = new string[] { "*.cs" };
		ClipboardCharacterLimit = 5000000; // Default to 5 million characters
	}

	private string GetInitialBasePath()
	{
		if (Environment.OSVersion.Platform == PlatformID.Win32NT)
		{
			var drives = System.IO.DriveInfo.GetDrives();
			return drives.Length > 0 ? drives[0].RootDirectory.FullName : "/";
		}
		else
		{
			return "/";
		}
	}
}