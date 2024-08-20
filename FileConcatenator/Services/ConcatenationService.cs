using System.Text;
using TextCopy;

namespace FileConcatenator;

public class ConcatenationService
{
	private readonly ConfigurationService _configurationManager;

	public ConcatenationService(ConfigurationService configurationManager)
	{
		_configurationManager = configurationManager;
	}

	public IEnumerable<string> GetDirectories(string path)
	{
		var directories = new List<string>();
		try
		{
			foreach (var dir in Directory.GetDirectories(path))
			{
				if (!_configurationManager.GetShowHiddenFiles() && (new DirectoryInfo(dir).Attributes & FileAttributes.Hidden) != 0)
				{
					continue;
				}
				directories.Add($"{Path.GetFileName(dir)}");
			}
		}
		catch (UnauthorizedAccessException)
		{
			directories.Add($"Error: Access to the path '{path}' is denied.");
		}
		catch (Exception ex)
		{
			directories.Add($"Error: {ex.Message}");
		}
		return directories;
	}

	public IEnumerable<string> GetFiles(string path)
	{
		var files = new List<string>();
		try
		{
			foreach (var file in Directory.GetFiles(path))
			{
				if (!_configurationManager.GetShowHiddenFiles() && (new FileInfo(file).Attributes & FileAttributes.Hidden) != 0)
				{
					continue;
				}
				files.Add($"{Path.GetFileName(file)}");
			}
		}
		catch (UnauthorizedAccessException)
		{
			files.Add($"Error: Access to the path '{path}' is denied.");
		}
		catch (Exception ex)
		{
			files.Add($"Error: {ex.Message}");
		}
		return files;
	}

	public (bool Success, string Message) ConcatenateFiles(string path)
	{
		var sb = new StringBuilder();
		bool accessDeniedFlag = false;

		foreach (var fileType in _configurationManager.GetTargetedFileTypes().Split(','))
		{
			try
			{
				var files = Directory.GetFiles(path, fileType.Trim(), SearchOption.AllDirectories);
				foreach (var file in files)
				{
					try
					{
						if (sb.Length > _configurationManager.GetClipboardCharacterLimit())
						{
							return (false, "Warning: Clipboard character limit reached. Not all files were concatenated.");
						}
						sb.AppendLine($"//{Path.GetFileName(file)}");
						sb.AppendLine(File.ReadAllText(file));
						sb.AppendLine();
					}
					catch (UnauthorizedAccessException)
					{
						accessDeniedFlag = true;
					}
				}
			}
			catch (UnauthorizedAccessException)
			{
				accessDeniedFlag = true;
			}
		}

		ClipboardService.SetText(sb.ToString());
		return accessDeniedFlag ? (true, "Note: Some files or directories could not be accessed and were skipped.\n") : (true, string.Empty);
	}
}
