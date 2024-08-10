using Spectre.Console;
using Spectre.Console.Rendering;

namespace FileConcatenator;

public class SpectreUI
{
	private Theme _theme;

	public SpectreUI(Theme theme)
	{
		_theme = theme;
	}

	public void SetTheme(Theme theme)
	{
		_theme = theme;
	}

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
		Text styledInput = new Text(input, _theme.TextColor);

		return AnsiConsole.Ask<string>(input);
	}
	public string Text(string text, Color? color = null)
	{
		return $"[bold {color ?? Color.White}]{text}[/]";
	}

	public string Header(string text, Color? color = null)
	{
		return $"[bold underline {color ?? Color.Grey78}]{text}[/]".ToUpper();
	}

	public IRenderable DisplayTree(string header, IEnumerable<string> items)
	{
		var tree = new Tree(header)
		{
			Style = new Style(foreground: _theme.TreeBranchColor)
		};
		foreach (var item in items)
		{
			tree.AddNode(Text(Markup.Escape(item), _theme.TextColor));
		}
		return tree;
	}

	public void MainLayout(string currentDirectory, string commands, string settingsHeaders, string currentSettings, IEnumerable<string> directoriesTree, IEnumerable<string> filesTree)
	{
		var rightTableColumn = new Table();
		rightTableColumn.AddColumn(new TableColumn(Header("Current Directory:", _theme.HeaderColor) + " " + Text(currentDirectory, _theme.AccentColor)));
		rightTableColumn.AddColumn(new TableColumn(""));
		rightTableColumn.AddRow(DisplayTree(Header("\nFolders:", _theme.HeaderColor), directoriesTree), DisplayTree(Header("\nFiles:", _theme.HeaderColor), filesTree));
		rightTableColumn.Border = TableBorder.None;

		var upperLeftColumn = new Table();
		upperLeftColumn.AddColumn(new TableColumn(Text(settingsHeaders, _theme.TextColor)));
		upperLeftColumn.AddColumn(new TableColumn(Text(currentSettings, _theme.AccentColor)));
		upperLeftColumn.Border = TableBorder.None;

		var lowerLeftColumn = new Table();
		lowerLeftColumn.AddColumn(new TableColumn(Text(commands, _theme.TextColor)));
		lowerLeftColumn.Border = TableBorder.None;

		var leftTableColumn = new Table();
		leftTableColumn.AddColumn(new TableColumn(Header("Current Settings:", _theme.HeaderColor)));
		leftTableColumn.AddRow(upperLeftColumn);
		leftTableColumn.AddRow(Header("Commands:", _theme.HeaderColor));
		leftTableColumn.AddRow(lowerLeftColumn);
		leftTableColumn.Border = TableBorder.None;
		leftTableColumn.Width(50);

		var mainLayout = new Table();
		mainLayout.AddColumn(new TableColumn(leftTableColumn));
		mainLayout.AddColumn(new TableColumn(rightTableColumn));
		mainLayout.Border = _theme.BorderType;
		mainLayout.BorderColor(_theme.BorderColor);

		AnsiConsole.Write(mainLayout);
	}
}