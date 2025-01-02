using Spectre.Console;
using Spectre.Console.Rendering;

public class SpectreUI
{
	#region Constants and Fields
	private const string BoldFormat = "bold";
	private const string UnderlineFormat = "underline";

	private readonly Color TextColor = Color.White;
	private readonly Color HeaderColor = Color.LightSkyBlue1;
	private readonly Color TreeColor = Color.RosyBrown;
	#endregion

	#region Public Interface Methods
	public void Clear() => AnsiConsole.Clear();

	public void ShowMessage(string message) => AnsiConsole.Write(message);

	public void ShowMessageAndWait(string message)
	{
		ShowMessage(message);
		Console.ReadKey();
	}

	public string GetInput(string prompt) => AnsiConsole.Ask<string>(prompt);
	#endregion

	#region Text Formatting
	public string Text(string content, Color? color = null)
		=> FormatText(content, color ?? TextColor, BoldFormat);

	public string Header(string content)
		=> FormatText(content, HeaderColor, BoldFormat, UnderlineFormat).ToUpper();

	private string FormatText(string content, Color color, params string[] formats)
		=> $"[{string.Join(" ", formats)} {color}]{content}[/]";
	#endregion

	#region Main Layout
	public void MainLayout(
		string currentDirectory,
		string commands,
		string settingsHeaders,
		string currentSettings,
		IEnumerable<string> directoriesTree,
		IEnumerable<string> filesTree)
	{
		var leftPanel = CreateLeftPanel(settingsHeaders, currentSettings, commands);
		var rightPanel = CreateRightPanel(currentDirectory, directoriesTree, filesTree);

		var mainLayout = new Table()
			.AddColumns(new TableColumn(leftPanel), new TableColumn(rightPanel))
			.BorderColor(TextColor)
			.Border(TableBorder.Horizontal);

		AnsiConsole.Write(mainLayout);
	}

	private Table CreateLeftPanel(string settingsHeaders, string currentSettings, string commands)
	{
		var settingsSection = new Table()
			.AddColumns(
				new TableColumn(Text(settingsHeaders)),
				new TableColumn(Text(currentSettings)))
			.Border(TableBorder.None);

		var commandsSection = new Table()
			.AddColumn(new TableColumn(Text(commands)))
			.Border(TableBorder.None);

		return new Table()
			.AddColumn(new TableColumn(Header("Current Settings:")))
			.AddRow(settingsSection)
			.AddEmptyRow()
			.AddRow(Header("Commands:"))
			.AddRow(commandsSection)
			.Border(TableBorder.None)
			.Width(50);
	}
	#endregion

	#region Directory Display
	public IRenderable DisplayTree(string header, IEnumerable<string> items)
	{
		var tree = new Tree(header) { Style = new Style(foreground: TreeColor) };
		foreach (var item in items)
		{
			tree.AddNode(Text(Markup.Escape(item)));
		}
		return tree;
	}

	private Panel CreateRightPanel(
		string currentDirectory,
		IEnumerable<string> directories,
		IEnumerable<string> files)
	{
		var pathPanel = CreateDirectoryPathPanel(currentDirectory);
		var contentTable = CreateDirectoryContentTable(directories, files);

		return new Panel(new Rows(pathPanel, contentTable))
		{
			BorderStyle = HeaderColor,
			Header = new PanelHeader("[[ Current Directory ]]".ToUpper()),
			Padding = new Padding(0, 1, 0, 0),
		};
	}

	private Panel CreateDirectoryPathPanel(string currentDirectory)
	{
		var textPath = new TextPath(currentDirectory.ToUpper())
			.SeparatorColor(TreeColor)
			.RootColor(TextColor)
			.StemColor(TextColor)
			.LeafColor(TextColor);

		return new Panel(textPath) { Border = BoxBorder.None };
	}

	private Table CreateDirectoryContentTable(IEnumerable<string> directories, IEnumerable<string> files)
	{
		var table = new Table { Border = TableBorder.Simple };

		table.AddColumn(new TableColumn(DisplayTree("Folders:".ToUpper(), directories)));
		table.AddColumn(new TableColumn(DisplayTree("Files:".ToUpper(), files)));

		table.Columns[0].Padding(0, 0);
		table.Columns[1].Padding(0, 0);

		return table;
	}
	#endregion
}