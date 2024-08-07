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

	public void MainLayout(string currentDirectory, string commands, string settings, IEnumerable<string> directoriesTree, IEnumerable<string> filesTree)
	{
		var currentDirectoryTable = new Table();
		currentDirectoryTable.AddColumn(new TableColumn(StyledText("Current Directory:").ToUpper() + " " + StyledText(currentDirectory, Color.White)));
		currentDirectoryTable.AddColumn(new TableColumn(""));
		currentDirectoryTable.AddRow(DisplayTree(StyledText("\nFolders:").ToUpper(), directoriesTree), (DisplayTree(StyledText("\nFiles:").ToUpper(), filesTree)));
		currentDirectoryTable.Border = TableBorder.None;

		var commandsTable = new Table();
		commandsTable.AddColumn(new TableColumn(StyledText("Current Settings:").ToUpper()));
		commandsTable.AddRow(StyledText(settings, Color.White));
		commandsTable.AddRow(StyledText(commands, Color.White));
		commandsTable.Border = TableBorder.None;
		commandsTable.Width(50);

		var mainLayout = new Table();
		mainLayout.AddColumn(new TableColumn(commandsTable));
		mainLayout.AddColumn(new TableColumn(currentDirectoryTable));
		mainLayout.Border = TableBorder.DoubleEdge;
		mainLayout.BorderColor(Color.Grey66);

		AnsiConsole.Write(mainLayout);
	}
}
