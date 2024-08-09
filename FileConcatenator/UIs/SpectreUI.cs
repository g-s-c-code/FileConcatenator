using Spectre.Console;
using Spectre.Console.Rendering;

namespace FileConcatenator;

public class SpectreUI
{
	private Theme _theme;

	public SpectreUI(Theme initialTheme)
	{
		_theme = initialTheme;
	}

	public void SetTheme(Theme newTheme)
	{
		_theme = newTheme;
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
		return AnsiConsole.Ask<string>(input);
	}
	public string StyledText(string text, Color? color = null)
	{
		return $"[bold {color ?? Color.White}]{text}[/]";
	}

	public string StyledHeader(string text, Color? color = null)
	{
		return $"[bold {color ?? Color.Grey78}]{text}[/]";
	}

	public IRenderable DisplayTree(string header, IEnumerable<string> items)
	{
		var tree = new Tree(header)
		{
			Style = new Style(foreground: _theme.TreeBranchColor)
		};
		foreach (var item in items)
		{
			tree.AddNode(StyledText(Markup.Escape(item), _theme.TextColor));
		}
		return tree;
	}

	public void MainLayout(string currentDirectory, string commands, string settingsHeaders, string currentSettings, IEnumerable<string> directoriesTree, IEnumerable<string> filesTree)
	{
		var rightTableColumn = new Table();
		rightTableColumn.AddColumn(new TableColumn(StyledHeader("Current Directory:").ToUpper() + " " + StyledText(currentDirectory.ToUpper(), _theme.AccentColor)));
		rightTableColumn.AddColumn(new TableColumn(""));
		rightTableColumn.AddRow(DisplayTree(StyledHeader("\nFolders:").ToUpper(), directoriesTree), DisplayTree(StyledHeader("\nFiles:").ToUpper(), filesTree));
		rightTableColumn.Border = TableBorder.None;

		var upperLeftColumn = new Table();
		upperLeftColumn.AddColumn(new TableColumn(StyledText(settingsHeaders, _theme.TextColor)));
		upperLeftColumn.AddColumn(new TableColumn(StyledText(currentSettings, _theme.AccentColor)));
		upperLeftColumn.Border = TableBorder.None;

		var lowerLeftColumn = new Table();
		lowerLeftColumn.AddColumn(new TableColumn(StyledText(commands, _theme.TextColor)));
		lowerLeftColumn.Border = TableBorder.None;

		var leftTableColumn = new Table();
		leftTableColumn.AddColumn(new TableColumn(StyledHeader("Current Settings:").ToUpper()));
		leftTableColumn.AddRow(upperLeftColumn);
		leftTableColumn.AddRow(StyledHeader("Commands:").ToUpper());
		leftTableColumn.AddRow(lowerLeftColumn);
		leftTableColumn.Border = TableBorder.None;
		leftTableColumn.Width(50);

		var mainLayout = new Table();
		mainLayout.AddColumn(new TableColumn(leftTableColumn));
		mainLayout.AddColumn(new TableColumn(rightTableColumn));
		mainLayout.Border = TableBorder.Square;
		mainLayout.BorderColor(_theme.BorderColor);

		AnsiConsole.Write(mainLayout);
	}
}