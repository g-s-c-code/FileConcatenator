﻿namespace FileConcatenator;

public class Configuration
{
	public bool ShowHiddenFiles { get; set; } = false;
	public int ClipboardCharacterLimit { get; set; } = 5000000;
	public string? BaseDirectoryPath { get; set; }
	public string? FileTypes { get; set; } = "*.cs";
	public string? Theme { get; set; } = "Default";
}
