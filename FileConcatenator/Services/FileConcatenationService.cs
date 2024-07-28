using System.Text;
using FileConcatenator.Models;
using TextCopy;

namespace FileConcatenator.Services;

public class FileConcatenationService
{
	private readonly Configuration config;

	public FileConcatenationService(Configuration config)
	{
		this.config = config;
	}

	public void DisplayFileTypes()
	{
		Console.WriteLine($"Currently Targeted File Types: {string.Join(", ", config.FileTypes)}");
	}

	public void DisplayDirectories(string path)
	{
		try
		{
			foreach (var dir in Directory.GetDirectories(path))
			{
				try
				{
					if (!config.ShowHiddenFiles && (new DirectoryInfo(dir).Attributes & FileAttributes.Hidden) != 0)
					{
						continue;
					}

					Console.WriteLine($"[D] {Path.GetFileName(dir)}");
				}
				catch (UnauthorizedAccessException)
				{
					Console.WriteLine($"[D] {Path.GetFileName(dir)} (Access Denied)");
				}
			}
		}
		catch (UnauthorizedAccessException)
		{
			Console.WriteLine($"Error: Access to the path '{path}' is denied.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex.Message}");
		}
		Console.WriteLine();
	}

	public void DisplayFiles(string path)
	{
		try
		{
			foreach (var file in Directory.GetFiles(path))
			{
				try
				{
					if (!config.ShowHiddenFiles && (new FileInfo(file).Attributes & FileAttributes.Hidden) != 0)
					{
						continue;
					}

					Console.WriteLine($"[F] {Path.GetFileName(file)}");
				}
				catch (UnauthorizedAccessException)
				{
					Console.WriteLine($"[F] {Path.GetFileName(file)} (Access Denied)");
				}
			}
		}
		catch (UnauthorizedAccessException)
		{
			Console.WriteLine($"Error: Access to the path '{path}' is denied.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex.Message}");
		}
		Console.WriteLine();
	}

	public void ConcatenateFilesAndCopyToClipboard(string path)
	{
		var sb = new StringBuilder();
		bool accessDeniedFlag = false;

		foreach (var fileType in config.FileTypes)
		{
			try
			{
				var files = Directory.GetFiles(path, fileType, SearchOption.AllDirectories);
				foreach (var file in files)
				{
					try
					{
						if (sb.Length > config.ClipboardCharacterLimit)
						{
							Console.WriteLine("Warning: Clipboard character limit reached. Not all files were concatenated.");
							goto ClipboardCopy;
						}
						sb.AppendLine($"//{Path.GetFileName(file)}");
						sb.AppendLine(File.ReadAllText(file));
						sb.AppendLine();
					}
					catch (UnauthorizedAccessException)
					{
						Console.WriteLine($"Error: Access to the file '{file}' is denied.");
						accessDeniedFlag = true;
					}
				}
			}
			catch (UnauthorizedAccessException)
			{
				Console.WriteLine($"Error: Access to some paths in '{path}' is denied.");
				accessDeniedFlag = true;
			}
		}

	ClipboardCopy:
		ClipboardService.SetText(sb.ToString());

		if (accessDeniedFlag)
		{
			Console.WriteLine("Note: Some files or directories could not be accessed and were skipped.");
		}
	}
}