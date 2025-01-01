public static class Constants
{
	public static class FileExtensions
	{
		public const string DefaultFileType = "*.cs";

		public static readonly IReadOnlySet<string> SupportedFileTypes = new HashSet<string>
		{
			"*.aspx", "*.bat", "*.c", "*.cc", "*.cfg", "*.cfm", "*.cgi", "*.class", "*.cmd",
			"*.com", "*.cpp", "*.cs", "*.css", "*.csv", "*.cxx", "*.dat", "*.db", "*.dbf", "*.env",
			"*.htm", "*.html", "*.ini", "*.java", "*.js", "*.json", "*.jsp", "*.jsx", "*.log",
			"*.m", "*.md", "*.php", "*.pl", "*.py", "*.razor", "*.rb", "*.sass", "*.scala", "*.scss",
			"*.sh", "*.sln", "*.sql", "*.swift", "*.tex", "*.ts", "*.vb", "*.vbs", "*.vcxproj",
			"*.xml", "*.yaml", "*.yml"
		};
	}

	public static class Commands
	{
		public const string ConcatenateAndCopy = "1";
		public const string SetClipboardLimit = "2";
		public const string SetFileTypes = "3";
		public const string SetBasePathManual = "4";
		public const string SetBasePathCurrent = "5";
		public const string ShowHiddenFiles = "6";
		public const string Help = "h";
		public const string Quit = "q";
		public const string ChangeDirectoryPrefix = "cd";
	}
}