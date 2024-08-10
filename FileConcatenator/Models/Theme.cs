using Spectre.Console;

namespace FileConcatenator;

public class Theme
{
	public Color TextColor { get; set; }
	public Color HeaderColor { get; set; }
	public Color TreeBranchColor { get; set; }
	public Color AccentColor { get; set; }
	public Color BorderColor { get; set; }

	public TableBorder BorderType { get; set; }

	public Theme(Color textColor, Color headerColor, Color treeBranchColor, Color accentColor, Color borderColor, TableBorder borderType)
	{
		TextColor = textColor;
		HeaderColor = headerColor;
		TreeBranchColor = treeBranchColor;
		AccentColor = accentColor;
		BorderColor = borderColor;
		BorderType = borderType;
	}
}
