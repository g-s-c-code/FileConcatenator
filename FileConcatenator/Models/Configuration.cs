public record Configuration
{
	public bool ShowHiddenFiles { get; set; } = false;
	public int ClipboardCharacterLimit { get; set; } = 5_000_000;
	public string BaseDirectoryPath { get; set; } = string.Empty;
	public string FileTypes { get; set; } = "*.cs";
}