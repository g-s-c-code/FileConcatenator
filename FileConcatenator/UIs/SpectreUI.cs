using Spectre.Console;
using Spectre.Console.Rendering;
using System.Collections.Generic;

namespace FileConcatenator
{
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

		public IRenderable DisplayPanel(string header, string content)
		{
			var displayPanel = new Panel(new Markup(content))
			{
				Header = new PanelHeader($"[bold] {header.ToUpper()} [/]"),
				Border = BoxBorder.None,
				Expand = true,
			};

			displayPanel.Padding(3, 1, 1, 1);

			return displayPanel;
		}

		public IRenderable DisplayTree(string header, IEnumerable<string> items)
		{
			var tree = new Tree($"").Style("grey78");
			foreach (var item in items)
			{
				tree.AddNode($"[grey78]{Markup.Escape(item)}[/]");
			}

			return new Panel(tree)
			{
				Header = new PanelHeader($"[bold] {header.ToUpper()} [/]"),
				Border = BoxBorder.None,
				Expand = true
			};
		}

		public void LayoutTable(IRenderable currentDirectory, IRenderable targetedFiles, IRenderable commands, IRenderable directories, IRenderable files)
		{
			var dirTable = new Table();
			dirTable.Border = TableBorder.None;

			dirTable.AddColumn(new TableColumn(directories));
			dirTable.AddColumn(new TableColumn(files));

			var table = new Table();
			table.Border = TableBorder.Rounded;

			table.AddColumn(new TableColumn(targetedFiles));
			table.AddColumn(new TableColumn(currentDirectory));

			table.AddRow(commands, dirTable);

			dirTable.Expand();
			table.Expand();

			AnsiConsole.Write(table);
		}

		//public void Layout(IRenderable currentDirectory, IRenderable targetedFiles, IRenderable commands, IRenderable directories, IRenderable files)
		//{
		//	var layout = new Layout()
		//		.SplitColumns(
		//			new Layout("Left")
		//				.SplitRows(
		//					new Layout("TargetedFiles"),
		//					new Layout("Commands")),
		//			new Layout("Right")
		//				.SplitRows(
		//					new Layout("CurrentDirectory"),
		//					new Layout("")
		//						.SplitColumns(
		//							new Layout("Directories"),
		//							new Layout("Files"))));

		//	//Content
		//	layout["TargetedFiles"].Update(targetedFiles);
		//	layout["CurrentDirectory"].Update(currentDirectory);
		//	layout["Commands"].Update(commands);
		//	layout["Directories"].Update(directories);
		//	layout["Files"].Update(files);


		//	//Sizing
		//	layout["Left"].Ratio(2);
		//	layout["Right"].Ratio(3);
		//	layout["TargetedFiles"].Size(3);
		//	layout["CurrentDirectory"].Size(3);


		//	AnsiConsole.Write(layout);
		//}
	}
}
