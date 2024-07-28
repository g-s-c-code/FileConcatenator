namespace FileConcatenator;

public class FileConcatenationController
{
	private readonly IUserInterface _ui;
	private readonly FileConcatenationService _fileConcatenationService;

	public FileConcatenationController(IUserInterface ui, FileConcatenationService fileConcatenationService)
	{
		_ui = ui;
		_fileConcatenationService = fileConcatenationService;
	}

	public void DisplayDirectories(string path)
	{
		var directories = _fileConcatenationService.GetDirectories(path);
		foreach (var dir in directories)
		{
			_ui.DisplayMessage(dir);
		}
	}

	public void DisplayFiles(string path)
	{
		var files = _fileConcatenationService.GetFiles(path);
		foreach (var file in files)
		{
			_ui.DisplayMessage(file);
		}
	}

	public void ConcatenateFilesAndCopyToClipboard(string path)
	{
		var result = _fileConcatenationService.ConcatenateFiles(path);
		_ui.DisplayMessage(result.Message);
		if (result.Success)
		{
			_ui.DisplayMessage("Files concatenated and copied to clipboard.");
		}
		else
		{
			_ui.DisplayMessage("Error occurred during concatenation.");
		}
	}
}
