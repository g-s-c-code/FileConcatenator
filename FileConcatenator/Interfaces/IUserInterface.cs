namespace FileConcatenator;

public interface IUserInterface
{
	void DisplayMessage(string message);
	string GetInput();
	void Clear();
	void DisplayDirectories(IEnumerable<string> files);
	void DisplayFiles(IEnumerable<string> files);
}