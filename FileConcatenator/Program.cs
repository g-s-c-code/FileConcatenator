using Spectre.Console;
using Microsoft.Extensions.DependencyInjection;

namespace FileConcatenator;

internal class Program
{
	public static readonly Dictionary<string, Theme> AvailableThemes = new Dictionary<string, Theme>
	{
		{"Default", new Theme(Color.White, Color.Grey78, Color.RosyBrown, Color.SteelBlue1_1, Color.Grey78)},
		{"Pastel", new Theme(Color.LightCyan3, Color.MistyRose1, Color.LightSteelBlue1, Color.PaleGreen3_1, Color.PaleVioletRed1)},
		{"Minimalistic", new Theme(Color.White, Color.White, Color.White, Color.White, Color.Black)}
	};

	private static void Main(string[] args)
	{
		var serviceCollection = new ServiceCollection();
		ConfigureServices(serviceCollection);

		var serviceProvider = serviceCollection.BuildServiceProvider();
		var programController = serviceProvider.GetService<Controller>();
		programController?.Run();
	}

	private static void ConfigureServices(IServiceCollection services)
	{
		services.AddSingleton<ConfigurationService>();
		services.AddSingleton(provider =>
		{
			var configurationService = provider.GetRequiredService<ConfigurationService>();
			var themeName = configurationService.GetSelectedTheme();
			return AvailableThemes.TryGetValue(themeName, out var theme) ? theme : AvailableThemes["Default"];
		});
		services.AddSingleton<SpectreUI>();
		services.AddSingleton<FileConcatenationService>();
		services.AddSingleton<Controller>();
	}
}
