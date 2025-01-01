using Spectre.Console;
using Spectre.Console.Rendering;

namespace FileConcatenator;

public class SpectreUI
{
	private const string BoldFormat = "bold";
	private const string UnderlineFormat = "underline";
	private readonly Color _defaultTextColor = Color.White;
	private readonly Color _defaultHeaderColor = Color.Grey78;
	private readonly Color _accentColor = Color.SteelBlue;

	public void Clear() => AnsiConsole.Clear();

	public void ShowMessage(string message) => AnsiConsole.Write(message);

	public void ShowMessageAndWait(string message)
	{
		ShowMessage(message);
		Console.ReadKey();
	}

	public string GetInput(string input) => AnsiConsole.Ask<string>(input);

	public string Text(string text, Color? color = null)
		=> FormatText(text, color ?? _defaultTextColor, BoldFormat);

	public string Header(string text, Color? color = null)
		=> FormatText(text, color ?? _defaultHeaderColor, BoldFormat, UnderlineFormat).ToUpper();

	private string FormatText(string text, Color color, params string[] formats)
		=> $"[{string.Join(" ", formats)} {color}]{text}[/]";

	public IRenderable DisplayTree(string header, IEnumerable<string> items)
	{
		var tree = new Tree(header)
		{
			Style = new Style(foreground: Color.Blue)
		};

		foreach (var item in items)
		{
			tree.AddNode(Text(Markup.Escape(item), Color.IndianRed));
		}

		return tree;
	}

	public void MainLayout(string currentDirectory, string commands, string settingsHeaders,
		string currentSettings, IEnumerable<string> directoriesTree, IEnumerable<string> filesTree)
	{
		var rightColumn = CreateRightColumn(currentDirectory, directoriesTree, filesTree);
		var leftColumn = CreateLeftColumn(settingsHeaders, currentSettings, commands);

		var mainLayout = new Table()
			.AddColumns(
				new TableColumn(leftColumn),
				new TableColumn(rightColumn))
			.BorderColor(_accentColor)
			.Border(TableBorder.Horizontal);

		AnsiConsole.Write(mainLayout);
	}

	private Table CreateRightColumn(string currentDirectory, IEnumerable<string> directoriesTree,
		IEnumerable<string> filesTree)
	{
		var directoryHeader = CreateHeaderWithContent("Current Directory:", currentDirectory);

		return new Table()
			.AddColumns(
				new TableColumn(directoryHeader),
				new TableColumn(""))
			.AddRow(
				DisplayTree(Header("\nFolders:", _accentColor), directoriesTree),
				DisplayTree(Header("\nFiles:", _accentColor), filesTree))
			.Border(TableBorder.None);
	}

	private Table CreateLeftColumn(string settingsHeaders, string currentSettings, string commands)
	{
		var upperSection = CreateSettingsSection(settingsHeaders, currentSettings);
		var lowerSection = CreateCommandsSection(commands);

		return new Table()
			.AddColumn(new TableColumn(Header("Current Settings:", _accentColor)))
			.AddRow(upperSection)
			.AddRow(Header("Commands:", _accentColor))
			.AddRow(lowerSection)
			.Border(TableBorder.None)
			.Width(50);
	}

	private Table CreateSettingsSection(string settingsHeaders, string currentSettings)
		=> new Table()
			.AddColumns(
				new TableColumn(Text(settingsHeaders, _accentColor)),
				new TableColumn(Text(currentSettings, _accentColor)))
			.Border(TableBorder.None);

	private Table CreateCommandsSection(string commands)
		=> new Table()
			.AddColumn(new TableColumn(Text(commands, _accentColor)))
			.Border(TableBorder.None);

	private string CreateHeaderWithContent(string headerText, string content)
		=> $"{Header(headerText, _accentColor)} {Text(content, _accentColor)}";
}