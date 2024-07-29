using Spectre.Console;

namespace FileConcatenator;

public class SpectreUI
{
	public void DisplayMessage(string message)
	{
		AnsiConsole.MarkupLine($"[white]{Markup.Escape(message)}[/]");
	}

	public string GetInput()
	{
		return AnsiConsole.Ask<string>("[bold white]Enter your input:[/]");
	}

	public void Clear()
	{
		AnsiConsole.Clear();
	}

	public void DisplayDirectories(IEnumerable<string> directories)
	{
		var headerPanel = new Panel("[bold white]Directories[/]")
			.Border(BoxBorder.Rounded);

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
		var headerPanel = new Panel("[bold white]Files[/]")
			.Border(BoxBorder.Rounded)
			;

		var fileTree = new Tree(string.Empty);
		foreach (var file in files)
		{
			fileTree.AddNode(Markup.Escape(file));
		}

		AnsiConsole.Write(headerPanel);
		AnsiConsole.Write(fileTree);
	}

	public void DisplayDirectoriesAndFiles(IEnumerable<string> directories, IEnumerable<string> files)
	{
		var directoryHeaderPanel = new Panel("[bold white]Directories[/]")
			.Border(BoxBorder.Rounded)
			;
		var fileHeaderPanel = new Panel("[bold white]Files[/]")
			.Border(BoxBorder.Rounded)
			;

		var directoryTree = new Tree(string.Empty);
		foreach (var dir in directories)
		{
			directoryTree.AddNode(Markup.Escape(dir));
		}

		var fileTree = new Tree(string.Empty);
		foreach (var file in files)
		{
			fileTree.AddNode(Markup.Escape(file));
		}

		AnsiConsole.Write(new Columns(
			directoryHeaderPanel,
			fileHeaderPanel
		));

		AnsiConsole.Write(new Columns(
			directoryTree,
			fileTree
		).PadRight(2).PadLeft(2));
	}

	public void Grid()
	{
		var grid = new Grid();

		// Add columns 
		grid.AddColumn();
		grid.AddColumn();
		grid.AddColumn();

		// Add header row 
		grid.AddRow(new Text[]{
	new Text("Header 1", new Style(Color.Red, Color.Black)).LeftJustified(),
	new Text("Header 3", new Style(Color.Blue, Color.Black)).RightJustified()
});

		// Add content row 
		grid.AddRow(new Text[]{
	new Text("Row 1").LeftJustified(),
	new Text("Row 3").RightJustified()
});

		// Write centered cell grid contents to Console
		AnsiConsole.Write(grid);
	}
}
