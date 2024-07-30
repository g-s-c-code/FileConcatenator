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
		return new Markup($"[bold white]{Markup.Escape(content)}[/]");
	}

	public IRenderable DisplayTree(string header, IEnumerable<string> items)
	{
		var tree = new Tree($"[bold white]{Markup.Escape(header.ToUpper())}[/]");
		foreach (var item in items)
		{
			tree.AddNode($"[grey82]{Markup.Escape(item)}[/]");
		}

		return tree;
	}

	public void MainLayout(IRenderable targetedFiles, IRenderable currentDirectory, IRenderable commands, IRenderable directoriesTree, IRenderable filesTree)
	{
		var mainLayout = new Table();

		var targetedFilesTable = new Table();
		targetedFilesTable.AddColumn(new TableColumn("Targeted Files:".ToUpper()));
		targetedFilesTable.AddColumn(new TableColumn(targetedFiles));
		targetedFilesTable.Border = TableBorder.None;

		var currentDirectoryTable = new Table();
		currentDirectoryTable.AddColumn(new TableColumn("Current Directory:".ToUpper()));
		currentDirectoryTable.AddColumn(new TableColumn(currentDirectory));
		currentDirectoryTable.Border = TableBorder.None;

		mainLayout.AddColumn(new TableColumn(targetedFilesTable));
		mainLayout.AddColumn(new TableColumn(currentDirectoryTable));

		var treeTable = new Table();
		treeTable.AddColumn(new TableColumn(directoriesTree));
		treeTable.AddColumn(new TableColumn(filesTree));

		mainLayout.AddRow(commands, treeTable);

		//Style
		mainLayout.Border = TableBorder.Double;
		treeTable.Border = TableBorder.None;

		AnsiConsole.Write(mainLayout);
	}
}
