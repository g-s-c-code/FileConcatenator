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

	public string GetInput(string prompt)
	{
		return AnsiConsole.Ask<string>(prompt);
	}

	public IRenderable DisplayMarkup(string content)
	{
		return new Markup($"[bold]{Markup.Escape(content)}[/]");
	}

	public IRenderable DisplayTree(string header, IEnumerable<string> items)
	{
		var tree = new Tree($"[bold]{Markup.Escape(header.ToUpper())}: [/]");
		foreach (var item in items)
		{
			tree.AddNode($"[bold white]{Markup.Escape(item)}[/]");
		}

		return tree;
	}

	public void MainLayout(IRenderable targetedFiles, IRenderable currentDirectory, IRenderable commands, IRenderable directoriesTree, IRenderable filesTree)
	{
		var targetedFilesTable = new Table();
		targetedFilesTable.AddColumn(new TableColumn("[bold]Targeted File Types: [/]".ToUpper()));
		targetedFilesTable.AddColumn(new TableColumn(targetedFiles));
		targetedFilesTable.Border = TableBorder.None;
		targetedFilesTable.Width(50);

		var currentDirectoryTable = new Table();
		currentDirectoryTable.AddColumn(new TableColumn("[bold]Current Directory: [/]".ToUpper()));
		currentDirectoryTable.AddColumn(new TableColumn(currentDirectory));
		currentDirectoryTable.Border = TableBorder.None;

		var commandsTable = new Table();
		commandsTable.AddColumn(new TableColumn("[bold]Commands: [/]".ToUpper()));
		commandsTable.AddRow(commands);
		commandsTable.Border = TableBorder.None;

		var directoryTreeTable = new Table();
		directoryTreeTable.AddColumn(new TableColumn(directoriesTree));
		directoryTreeTable.AddColumn(new TableColumn(filesTree));
		directoryTreeTable.Border = TableBorder.None;
		directoryTreeTable.Collapse();

		var mainLayout = new Table();
		mainLayout.AddColumn(new TableColumn(targetedFilesTable));
		mainLayout.AddColumn(new TableColumn(currentDirectoryTable));
		mainLayout.AddRow(commandsTable, directoryTreeTable);
		mainLayout.Border = TableBorder.Double;

		AnsiConsole.Write(mainLayout);
	}
}
