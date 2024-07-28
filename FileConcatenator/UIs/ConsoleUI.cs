namespace FileConcatenator;

public class ConsoleUI : IUserInterface
{
	public void DisplayMessage(string message)
	{
		Console.WriteLine(message);
	}

	public string GetInput()
	{
		return Console.ReadLine() ?? "";
	}

	public void Clear()
	{
		Console.Clear();
	}
}
