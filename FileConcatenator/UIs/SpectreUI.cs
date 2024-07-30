using Spectre.Console;
using Spectre.Console.Rendering;

namespace FileConcatenator;

public class SpectreUI
{
	private readonly ConfigurationManager _configurationManager;
	private readonly FileConcatenationService _fileConcatenationService;

	public SpectreUI(ConfigurationManager configurationManager, FileConcatenationService fileConcatenationService)
	{
		_configurationManager = configurationManager;
		_fileConcatenationService = fileConcatenationService;
	}

	public void Clear()
	{
		AnsiConsole.Clear();
	}
	public IRenderable DisplayCurrentDirectory()
	{
		return new Panel(new Markup(_configurationManager.GetBaseDirectoryPath()))
		{
			Header = new PanelHeader("Current Directory"),
			Border = BoxBorder.Rounded,
			Expand = true
		};
	}

	public IRenderable DisplayTargetedFiles()
	{
		return new Panel(new Markup(_configurationManager.GetTargetedFileTypes()))
		{
			Header = new PanelHeader("Targeted File Types"),
			Border = BoxBorder.Rounded,
			Expand = true
		};
	}

	public void Layout()
	{
		var layout = new Layout()
			.SplitColumns(
				new Layout("Left")
					.SplitRows(
						new Layout("Information")
							.SplitRows(
								new Layout("CurrentDirectory"),
								new Layout("TargetedFiles")),
						new Layout("Commands")),
				new Layout("Right").SplitColumns(
					new Layout("Directories"),
					new Layout("Files")));

		layout["CurrentDirectory"].Update(DisplayCurrentDirectory());
		layout["TargetedFiles"].Update(DisplayTargetedFiles());
		layout["Directories"].Update(RenderDirectories());
		layout["Files"].Update(RenderFiles());
		layout["Left"].Ratio(1);
		layout["Right"].Ratio(2);

		AnsiConsole.Write(layout);
	}

	private IRenderable RenderDirectories()
	{
		var directories = GetDirectories();
		var panel = new Panel(GetDirectoryTree(directories))
		{
			Header = new PanelHeader(""),
			Expand = true
		};
		return panel;
	}

	private IRenderable RenderFiles()
	{
		var files = GetFiles();
		var panel = new Panel(GetFileTree(files))
		{
			Header = new PanelHeader(""),
			Expand = true
		};
		return panel;
	}

	private Tree GetDirectoryTree(IEnumerable<string> directories)
	{
		var tree = new Tree("[bold white]Directories[/]");
		foreach (var dir in directories)
		{
			tree.AddNode(Markup.Escape(dir));
		}

		return tree;
	}

	private Tree GetFileTree(IEnumerable<string> files)
	{
		var tree = new Tree("[bold white]Files[/]");
		foreach (var file in files)
		{
			tree.AddNode(Markup.Escape(file));
		}

		return tree;
	}

	private IEnumerable<string> GetDirectories()
	{
		var path = _configurationManager.GetBaseDirectoryPath();
		return _fileConcatenationService.GetDirectories(path);
	}

	public IEnumerable<string> GetFiles()
	{
		var path = _configurationManager.GetBaseDirectoryPath();
		return _fileConcatenationService.GetFiles(path);
	}

	public void DisplayDirectories(IEnumerable<string> directories)
	{
		var headerPanel = new Panel("[bold white]Directories[/]");
		var directoryTree = new Tree(string.Empty);
		foreach (var dir in directories)
		{
			directoryTree.AddNode(Markup.Escape(dir));
		}
		AnsiConsole.Write(headerPanel);
		AnsiConsole.Write(directoryTree);
	}
	public void DisplayFiles(IEnumerable<string> files)
	{
		var headerPanel = new Panel("[bold white]Files[/]");
		var fileTree = new Tree(string.Empty);
		foreach (var file in files)
		{
			fileTree.AddNode(Markup.Escape(file));
		}
		AnsiConsole.Write(headerPanel);
		AnsiConsole.Write(fileTree);
	}

	public string GetInput()
	{
		return AnsiConsole.Ask<string>("Enter command: ");
	}

	public void Header(string message)
	{
		AnsiConsole.MarkupLine(message);
	}
}
