using Spectre.Console;
using Spectre.Console.Rendering;
public class SpectreUI
{
	private const string BoldFormat = "bold";
	private const string UnderlineFormat = "underline";
	private readonly Color _textColor = Color.White;
	private readonly Color _headerColor = Color.LightSkyBlue1;

	public void Clear() => AnsiConsole.Clear();
	public void ShowMessage(string message) => AnsiConsole.Write(message);
	public void ShowMessageAndWait(string message)
	{
		ShowMessage(message);
		Console.ReadKey();
	}
	public string GetInput(string input) => AnsiConsole.Ask<string>(input);

	public string Text(string text, Color? color = null)
		=> FormatText(text, color ?? _textColor, BoldFormat);

	public string Header(string text)
		=> FormatText(text, _headerColor, BoldFormat, UnderlineFormat).ToUpper();

	private string FormatText(string text, Color color, params string[] formats)
		=> $"[{string.Join(" ", formats)} {color}]{text}[/]";

	public IRenderable DisplayTree(string header, IEnumerable<string> items)
	{
		var tree = new Tree(header)
		{
			Style = new Style(foreground: Color.RosyBrown)
		};
		foreach (var item in items)
		{
			tree.AddNode(Text(Markup.Escape(item)));
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
			.BorderColor(_textColor)
			.Border(TableBorder.Horizontal);
		AnsiConsole.Write(mainLayout);
	}

	private Table CreateRightColumn(string currentDirectory, IEnumerable<string> directoriesTree, IEnumerable<string> filesTree)
	{
		return new Table()
			.AddColumns(
				new TableColumn(CurrentDirectoryPathUI()),
				new TableColumn(""))
			.AddRow(
				DisplayTree(Header("\nFolders:"), directoriesTree),
				DisplayTree(Header("\nFiles:"), filesTree))
			.Border(TableBorder.None);
	}

	private Panel CurrentDirectoryPathUI()
	{
		var currentDirectory = new TextPath(Directory.GetCurrentDirectory().ToUpper())
			.SeparatorColor(Color.RosyBrown)
			.RootColor(_textColor)
			.StemColor(_textColor)
			.LeafColor(_textColor);

		return new Panel(currentDirectory)
		{
			Padding = new Padding(0),
			Border = BoxBorder.None,
		};
	}

	private Table CreateLeftColumn(string settingsHeaders, string currentSettings, string commands)
	{
		var upperSection = CreateSettingsSection(settingsHeaders, currentSettings);
		var lowerSection = CreateCommandsSection(commands);

		return new Table()
			.AddColumn(new TableColumn(Header("Current Settings:")))
			.AddRow(upperSection)
			.AddEmptyRow()
			.AddRow(Header("Commands:"))
			.AddRow(lowerSection)
			.Border(TableBorder.None)
			.Width(50);
	}

	private Table CreateSettingsSection(string settingsHeaders, string currentSettings)
		=> new Table()
			.AddColumns(
				new TableColumn(Text(settingsHeaders)),
				new TableColumn(Text(currentSettings)))
			.Border(TableBorder.None);

	private Table CreateCommandsSection(string commands)
		=> new Table()
			.AddColumn(new TableColumn(Text(commands)))
			.Border(TableBorder.None);
}