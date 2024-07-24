using System;
using System.IO;
using System.Text;
using TextCopy;

namespace FileConcatenator
{
	class Program
	{
		static void Main(string[] args)
		{
			string currentDirectory = Directory.GetCurrentDirectory();

			while (true)
			{
				Console.Clear();
				Console.WriteLine($"Current Directory: {currentDirectory}");
				Console.WriteLine("Directories:");
				DisplayDirectories(currentDirectory);

				Console.WriteLine("Files:");
				DisplayFiles(currentDirectory);

				Console.WriteLine("\nCommands:");
				Console.WriteLine("[cd <directory>] - Change Directory");
				Console.WriteLine("[1] - Concatenate .cs files and copy to clipboard");
				Console.WriteLine("[2] - Exit application");

				Console.Write("\nEnter command: ");
				string command = Console.ReadLine();

				if (command.StartsWith("cd"))
				{
					string[] parts = command.Split(' ', 2);
					if (parts.Length == 2)
					{
						string newDirectory = Path.Combine(currentDirectory, parts[1]);
						if (Directory.Exists(newDirectory))
						{
							currentDirectory = newDirectory;
						}
						else
						{
							Console.WriteLine("Directory does not exist.");
							Console.ReadKey();
						}
					}
				}
				else if (command == "1")
				{
					ConcatenateFilesAndCopyToClipboard(currentDirectory);
					Console.WriteLine("Files concatenated and copied to clipboard.");
					Console.ReadKey();
				}
				else if (command == "2")
				{
					break;
				}
				else
				{
					Console.WriteLine("Invalid command.");
					Console.ReadKey();
				}
			}
		}

		static void DisplayDirectories(string path)
		{
			try
			{
				foreach (var dir in Directory.GetDirectories(path))
				{
					Console.WriteLine($"[D] {Path.GetFileName(dir)}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
			}
		}

		static void DisplayFiles(string path)
		{
			try
			{
				foreach (var file in Directory.GetFiles(path))
				{
					Console.WriteLine($"[F] {Path.GetFileName(file)}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
			}
		}

		static void ConcatenateFilesAndCopyToClipboard(string path)
		{
			var sb = new StringBuilder();
			var files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);

			foreach (var file in files)
			{
				sb.AppendLine(File.ReadAllText(file));
				sb.AppendLine();
			}

			ClipboardService.SetText(sb.ToString());
		}
	}
}