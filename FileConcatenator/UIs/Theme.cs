using Spectre.Console;

namespace FileConcatenator;

public class Theme
{
	public Color PrimaryColor { get; set; }
	public Color SecondaryColor { get; set; }
	public Color AccentColor { get; set; }
	public Color TextColor { get; set; }
	public Color HeaderColor { get; set; }

	public Theme(Color primary, Color secondary, Color accent, Color text, Color header)
	{
		PrimaryColor = primary;
		SecondaryColor = secondary;
		AccentColor = accent;
		TextColor = text;
		HeaderColor = header;
	}
}