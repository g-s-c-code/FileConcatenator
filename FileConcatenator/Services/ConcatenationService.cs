using System.Text;
using TextCopy;

public class ConcatenationService
{
	private readonly ConfigurationService _configurationService;
	private const string FileNotAccessibleMessage = "Error: Access to the path '{0}' is denied.";

	public ConcatenationService(ConfigurationService configurationService)
	{
		_configurationService = configurationService;
	}

	public IReadOnlyCollection<string> GetDirectories(string path)
	{
		try
		{
			return Directory.GetDirectories(path)
				.Where(dir => ShouldIncludeItem(dir))
				.Select(Path.GetFileName)
				.ToList();
		}
		catch (UnauthorizedAccessException)
		{
			return new[] { string.Format(FileNotAccessibleMessage, path) };
		}
		catch (Exception ex)
		{
			return new[] { $"Error: {ex.Message}" };
		}
	}

	public IReadOnlyCollection<string> GetFiles(string path)
	{
		try
		{
			return Directory.GetFiles(path)
				.Where(file => ShouldIncludeItem(file))
				.Select(Path.GetFileName)
				.ToList();
		}
		catch (UnauthorizedAccessException)
		{
			return new[] { string.Format(FileNotAccessibleMessage, path) };
		}
		catch (Exception ex)
		{
			return new[] { $"Error: {ex.Message}" };
		}
	}

	public (bool Success, string Message) ConcatenateFiles(string path)
	{
		var contentBuilder = new StringBuilder();
		var accessDeniedOccurred = false;

		foreach (var fileType in _configurationService.FileTypes.Split(','))
		{
			if (!TryProcessFilesOfType(path, fileType.Trim(), contentBuilder, ref accessDeniedOccurred))
			{
				return (false, "Warning: Clipboard character limit reached. Not all files were concatenated.");
			}
		}

		ClipboardService.SetText(contentBuilder.ToString());
		return accessDeniedOccurred
			? (true, "Note: Some files or directories could not be accessed and were skipped.\n")
			: (true, string.Empty);
	}

	private bool TryProcessFilesOfType(string path, string fileType, StringBuilder contentBuilder, ref bool accessDeniedOccurred)
	{
		try
		{
			var files = Directory.GetFiles(path, fileType, SearchOption.AllDirectories);
			foreach (var file in files)
			{
				if (!TryAppendFileContent(file, contentBuilder))
				{
					accessDeniedOccurred = true;
					continue;
				}

				if (contentBuilder.Length > _configurationService.ClipboardCharacterLimit)
				{
					return false;
				}
			}
		}
		catch (UnauthorizedAccessException)
		{
			accessDeniedOccurred = true;
		}

		return true;
	}

	private bool TryAppendFileContent(string filePath, StringBuilder contentBuilder)
	{
		try
		{
			contentBuilder.AppendLine($"//{Path.GetFileName(filePath)}");
			contentBuilder.AppendLine(File.ReadAllText(filePath));
			contentBuilder.AppendLine();
			return true;
		}
		catch (UnauthorizedAccessException)
		{
			return false;
		}
	}

	private bool ShouldIncludeItem(string path)
	{
		return _configurationService.ShowHiddenFiles ||
			   (File.GetAttributes(path) & FileAttributes.Hidden) == 0;
	}
}