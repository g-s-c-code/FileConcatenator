// SpectreUI.cs
using Spectre.Console;

namespace FileConcatenator;

public class SpectreUI : IUserInterface
{
	public void DisplayMessage(string message)
	{
		AnsiConsole.MarkupLine($"[bold white]{Markup.Escape(message)}[/]");
	}

	public string GetInput()
	{
		return AnsiConsole.Ask<string>("[bold white]Enter your input:[/]");
	}

	public void Clear()
	{
		AnsiConsole.Clear();
	}

	public void DisplayDirectories(IEnumerable<string> directories)
	{
		var root = new Tree("[bold white]Directories[/]");

		foreach (var dir in directories)
		{
			root.AddNode(Markup.Escape(dir));
		}

		AnsiConsole.Write(root);
	}

	public void DisplayFiles(IEnumerable<string> files)
	{
		var root = new Tree("[bold white]Files[/]");

		foreach (var file in files)
		{
			root.AddNode(Markup.Escape(file));
		}

		AnsiConsole.Write(root);
	}

	public void DisplayDirectoriesAndFiles(IEnumerable<string> directories, IEnumerable<string> files)
	{
		var directoryTree = new Tree("[bold white]Directories[/]");
		foreach (var dir in directories)
		{
			directoryTree.AddNode(Markup.Escape(dir));
		}

		var fileTree = new Tree("[bold white]Files[/]");
		foreach (var file in files)
		{
			fileTree.AddNode(Markup.Escape(file));
		}

		AnsiConsole.Write(
			new Columns(
				new Panel(directoryTree).Expand(),
				new Panel(fileTree).Expand()
			)
			.Expand()
			.PadRight(2)
			.PadLeft(2)
		);
	}
}
