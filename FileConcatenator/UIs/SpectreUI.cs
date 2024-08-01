using Spectre.Console;
using Spectre.Console.Rendering;

namespace FileConcatenator;

public class SpectreUI
{
	public void Clear()
	{
		AnsiConsole.Clear();
	}

	public void ShowMessage(string message)
	{
		AnsiConsole.Write(message);
	}

	public void ShowMessageAndWait(string message)
	{
		AnsiConsole.Write(message);
		Console.ReadKey();
	}

	public string GetInput(string input)
	{
		return AnsiConsole.Ask<string>(input);
	}

	public string StyledText(string text, Color? color = null)
	{
		color ??= Color.Grey78;
		return $"[bold {color}]{text}[/]";
	}


	public IRenderable DisplayTree(string header, IEnumerable<string> items)
	{
		var tree = new Tree(header)
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
		targetedFilesTable.AddColumn(new TableColumn(StyledText("Targeted File Types:").ToUpper()));
		targetedFilesTable.AddColumn(new TableColumn(StyledText(targetedFiles, Color.SteelBlue1_1)));
		targetedFilesTable.Border = TableBorder.None;

		var currentDirectoryTable = new Table();
		currentDirectoryTable.AddColumn(new TableColumn(StyledText("Directory:").ToUpper()));
		currentDirectoryTable.AddColumn(new TableColumn(StyledText(currentDirectory, Color.SteelBlue1_1)));
		currentDirectoryTable.Border = TableBorder.None;

		var commandsTable = new Table();
		commandsTable.AddColumn(new TableColumn(StyledText("Commands:").ToUpper()));
		commandsTable.AddRow(StyledText(commands, Color.White));
		commandsTable.Border = TableBorder.None;
		commandsTable.Width(50);


		var directoryTreeTable = new Table();
		directoryTreeTable.AddColumn(new TableColumn(DisplayTree(StyledText("Folders:").ToUpper(), directoriesTree)));
		directoryTreeTable.AddColumn(new TableColumn(DisplayTree(StyledText("Files:").ToUpper(), filesTree)));
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
}
