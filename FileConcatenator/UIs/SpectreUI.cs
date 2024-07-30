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

		public IRenderable DisplayPanel(string header, string content)
		{
			return new Panel(new Markup(content))
			{
				Header = new PanelHeader($"[bold] {header} [/]").Centered(),
				Border = BoxBorder.Rounded,
				Expand = true
			};
		}

		public IRenderable DisplayTree(string header, IEnumerable<string> items)
		{
			var tree = new Tree($"[bold white]{header}[/]");
			foreach (var item in items)
			{
				tree.AddNode(Markup.Escape(item));
			}
			return tree;
		}

		public void Layout(IRenderable currentDirectory, IRenderable targetedFiles, IRenderable commands, IRenderable directories, IRenderable files)
		{
			var layout = new Layout()
				.SplitColumns(
					new Layout("Left")
						.SplitRows(
							new Layout("Information")
								.SplitRows(
									new Layout("CurrentDirectory"),
									new Layout("TargetedFiles")),
							new Layout("Commands")),
					new Layout("Right").SplitColumns(
						new Layout("Directories"),
						new Layout("Files")));

			layout["CurrentDirectory"].Update(currentDirectory);
			layout["TargetedFiles"].Update(targetedFiles);
			layout["Commands"].Update(commands);
			layout["Directories"].Update(directories);
			layout["Files"].Update(files);
			layout["Left"].Ratio(2);
			layout["Right"].Ratio(3);

			AnsiConsole.Write(layout);
		}

		public void DisplayMessage(string message)
		{
			AnsiConsole.MarkupLine(message);
		}

		public string GetInput(string prompt)
		{
			return AnsiConsole.Ask<string>(prompt);
		}
	}
}
