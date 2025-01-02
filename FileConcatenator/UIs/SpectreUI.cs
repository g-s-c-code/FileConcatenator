using Spectre.Console;
using Spectre.Console.Rendering;

public class SpectreUI
{
	private const string BoldFormat = "bold";
	private const string UnderlineFormat = "underline";
	private readonly Color TextColor = Color.White;
	private readonly Color HeaderColor = Color.LightSkyBlue1;

	// Utility Methods
	public void Clear() => AnsiConsole.Clear();

	public void ShowMessage(string message) => AnsiConsole.Write(message);

	public void ShowMessageAndWait(string message)
	{
		ShowMessage(message);
		Console.ReadKey();
	}

	public string GetInput(string prompt) => AnsiConsole.Ask<string>(prompt);

	// Formatting Methods
	public string Text(string content, Color? color = null)
		=> FormatText(content, color ?? TextColor, BoldFormat);

	public string Header(string content)
		=> FormatText(content, HeaderColor, BoldFormat, UnderlineFormat).ToUpper();

	private string FormatText(string content, Color color, params string[] formats)
		=> $"[{string.Join(" ", formats)} {color}]{content}[/]";

	// Rendering Methods
	public void MainLayout(string currentDirectory, string commands, string settingsHeaders,
		string currentSettings, IEnumerable<string> directoriesTree, IEnumerable<string> filesTree)
	{
		var leftColumn = CreateLeftColumn(settingsHeaders, currentSettings, commands);
		var rightColumn = DirectoryContentUI(CurrentDirectoryPathUI(currentDirectory), CurrentDirectoryContentUI(directoriesTree, filesTree));

		var mainLayout = new Table()
			.AddColumns(
				new TableColumn(leftColumn),
				new TableColumn(rightColumn))
			.BorderColor(TextColor)
			.Border(TableBorder.Horizontal);

		AnsiConsole.Write(mainLayout);
	}

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

	// Private Helper Methods
	private Table CreateLeftColumn(string settingsHeaders, string currentSettings, string commands)
	{
		var settingsSection = CreateSettingsSection(settingsHeaders, currentSettings);
		var commandsSection = CreateCommandsSection(commands);

		return new Table()
			.AddColumn(new TableColumn(Header("Current Settings:")))
			.AddRow(settingsSection)
			.AddEmptyRow()
			.AddRow(Header("Commands:"))
			.AddRow(commandsSection)
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

	private Table CreateRightColumn(string currentDirectory, IEnumerable<string> directoriesTree, IEnumerable<string> filesTree)
	{
		return new Table()
			.AddColumns(
				new TableColumn(CurrentDirectoryPathUI(currentDirectory)),
				new TableColumn(""))
			.AddRow(
				DisplayTree(Header("\nFolders:"), directoriesTree),
				DisplayTree(Header("\nFiles:"), filesTree))
			.Border(TableBorder.None);
	}

	private Panel CurrentDirectoryPathUI(string currentDirectory)
	{
		var textPath = new TextPath(currentDirectory.ToUpper())
			.SeparatorColor(Color.RosyBrown)
			.RootColor(TextColor)
			.StemColor(TextColor)
			.LeafColor(TextColor);

		return new Panel(textPath)
		{
			Border = BoxBorder.None,
		};
	}

	private Panel DirectoryContentUI(Panel directoryPathPanel, Table directoryContentTable)
	{
		return new Panel(new Rows(directoryPathPanel, directoryContentTable))
		{
			BorderStyle = Color.LightSkyBlue1,
			Header = new PanelHeader("[[ Current Directory ]]".ToUpper()),
			Padding = new Padding(0, 1, 0, 0),
		};
	}

	private Table CurrentDirectoryContentUI(IEnumerable<string> directories, IEnumerable<string> files)
	{
		var table = new Table
		{
			Border = TableBorder.Simple,
		};

		table.AddColumn(new TableColumn(DisplayTree("Folders:".ToUpper(), directories)));
		table.AddColumn(new TableColumn(DisplayTree("Files:".ToUpper(), files)));
		table.Columns[0].Padding(0, 0);
		table.Columns[1].Padding(0, 0);

		return table;
	}
}
