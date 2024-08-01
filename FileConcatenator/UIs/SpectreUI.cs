using System.Reflection.PortableExecutable;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace FileConcatenator;

public class SpectreUI
{
	public void Clear()
	{
		AnsiConsole.Clear();
	}

	public void DisplayMessage(string message)
	{
		AnsiConsole.MarkupLine(message);
	}

	public string GetInput(string input)
	{
		return AnsiConsole.Ask<string>(input);
	}

	public IRenderable DisplayTree(string header, IEnumerable<string> items)
	{
		var tree = new Tree($"[bold grey82]{Markup.Escape(header.ToUpper())}:[/]")
		{
			Style = new Style(foreground: Color.RosyBrown)
		};

		foreach (var item in items)
		{
			tree.AddNode($"[bold white]{Markup.Escape(item)}[/]");
		}

		return tree;
	}

	public void MainLayout(string targetedFiles, string currentDirectory, string commands, IEnumerable<string> directoriesTree, IEnumerable<string> filesTree)
	{
		var targetedFilesTable = new Table();
		targetedFilesTable.AddColumn(new TableColumn(StyledHeader("Targeted File Types")));
		targetedFilesTable.AddColumn(new TableColumn(targetedFiles));
		targetedFilesTable.Border = TableBorder.None;

		var currentDirectoryTable = new Table();
		currentDirectoryTable.AddColumn(new TableColumn(StyledHeader("Directory")));
		currentDirectoryTable.AddColumn(new TableColumn(currentDirectory));
		currentDirectoryTable.Border = TableBorder.None;

		var commandsTable = new Table();
		commandsTable.AddColumn(new TableColumn(StyledHeader("Commands")));
		commandsTable.AddRow(commands);
		commandsTable.Border = TableBorder.None;
		commandsTable.Width(50);


		var directoryTreeTable = new Table();
		directoryTreeTable.AddColumn(new TableColumn(DisplayTree(StyledHeader("Folders"), directoriesTree)));
		directoryTreeTable.AddColumn(new TableColumn(DisplayTree(StyledHeader("Files"), filesTree)));
		directoryTreeTable.Border = TableBorder.None;
		directoryTreeTable.Collapse();

		var mainLayout = new Table();
		mainLayout.AddColumn(new TableColumn(targetedFilesTable));
		mainLayout.AddColumn(new TableColumn(currentDirectoryTable));
		mainLayout.AddRow(commandsTable, directoryTreeTable);
		mainLayout.Border = TableBorder.DoubleEdge;
		mainLayout.BorderColor(Color.Grey66);

		AnsiConsole.Write(mainLayout);
	}

	public string StyledHeader(string header)
	{
		return $"[bold grey82]{header}[/]".ToUpper();
	}
}
