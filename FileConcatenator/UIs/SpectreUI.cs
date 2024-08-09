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
		color ??= Color.White;
		return $"[bold {color}]{text}[/]";
	}

	public string StyledHeader(string text, Color? color = null)
	{
		color ??= Color.Grey78;
		return $"[bold underline {color}]{text}[/]";
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

	public void MainLayout(string currentDirectory, string commands, string settingsHeaders, string currentSettings, IEnumerable<string> directoriesTree, IEnumerable<string> filesTree)
	{
		var currentDirectoryTable = new Table();
		currentDirectoryTable.AddColumn(new TableColumn(StyledHeader("Current Directory:").ToUpper() + " " + StyledText(currentDirectory.ToUpper(), Color.SteelBlue1_1)));
		currentDirectoryTable.AddColumn(new TableColumn(""));
		currentDirectoryTable.AddRow(DisplayTree(StyledHeader("\nFolders:").ToUpper(), directoriesTree), DisplayTree(StyledHeader("\nFiles:").ToUpper(), filesTree));
		currentDirectoryTable.Border = TableBorder.None;

		var settingsTable = new Table();
		settingsTable.AddColumn(new TableColumn(StyledText(settingsHeaders, Color.White)));
		settingsTable.AddColumn(new TableColumn(StyledText(currentSettings, Color.White)));
		settingsTable.Border = TableBorder.None;

		var commandsTable = new Table();
		commandsTable.AddColumn(new TableColumn(StyledHeader("Current Settings:").ToUpper()));
		commandsTable.AddRow(settingsTable);
		commandsTable.AddRow(StyledText(commands, Color.White));
		commandsTable.Border = TableBorder.None;
		commandsTable.Width(50);

		var mainLayout = new Table();
		mainLayout.AddColumn(new TableColumn(commandsTable));
		mainLayout.AddColumn(new TableColumn(currentDirectoryTable));
		mainLayout.Border = TableBorder.Minimal;
		mainLayout.BorderColor(Color.White);

		AnsiConsole.Write(mainLayout);
	}
}
