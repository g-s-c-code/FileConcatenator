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

	public void LayoutTable(IRenderable firstColumnFirstRow, IRenderable secondColumnFirstRow, IRenderable firstColumnSecondRow, IRenderable directories, IRenderable files)
	{
		var table = new Table();
		table.AddColumn(new TableColumn(firstColumnFirstRow));
		table.AddColumn(new TableColumn(secondColumnFirstRow));

		var dirTable = new Table();
		dirTable.AddColumn(new TableColumn(directories));
		dirTable.AddColumn(new TableColumn(files));

		table.AddRow(firstColumnSecondRow, dirTable);

		//Style
		table.Border = TableBorder.Double;
		dirTable.Border = TableBorder.None;
		table.Columns[0].Alignment = Justify.Left;

		AnsiConsole.Write(table);
	}
}
