using Spectre.Console;

namespace FileConcatenator;

public class SpectreUI : IUserInterface
{
	public void DisplayMessage(string message)
	{
		AnsiConsole.MarkupLine($"[bold yellow]{Markup.Escape(message)}[/]");
	}

	public string GetInput()
	{
		return AnsiConsole.Ask<string>("Enter your input:");
	}

	public void Clear()
	{
		AnsiConsole.Clear();
	}

	public void DisplayDirectories(IEnumerable<string> directories)
	{
		var table = new Table();
		table.AddColumn("Directories");

		foreach (var dir in directories)
		{
			table.AddRow(Markup.Escape(dir));
		}

		AnsiConsole.Write(table);
	}


	public void DisplayFiles(IEnumerable<string> files)
	{
		var table = new Table();
		table.AddColumn("Files");

		foreach (var file in files)
		{
			// Use Markup.Escape to handle any special characters
			table.AddRow(Markup.Escape(file));
		}

		AnsiConsole.Write(table);
	}
}
