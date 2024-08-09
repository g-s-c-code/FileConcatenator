using FileConcatenator;
using Spectre.Console;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
	private static readonly Dictionary<string, Theme> AvailableThemes = new Dictionary<string, Theme>
	{
		{"Default", new Theme(Color.White, Color.Grey78, Color.RosyBrown, Color.SteelBlue1_1, Color.Grey78)},
		{"Dark", new Theme(Color.DarkSlateGray2, Color.SlateBlue1, Color.DodgerBlue1, Color.White, Color.SlateBlue1)},
		{"Light", new Theme(Color.White, Color.Grey, Color.RoyalBlue1, Color.White, Color.DarkSlateGray1)}
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
		services.AddSingleton<ConfigurationManager>();
		services.AddSingleton(provider =>
		{
			var configManager = provider.GetRequiredService<ConfigurationManager>();
			var themeName = configManager.GetSelectedTheme();
			return AvailableThemes.TryGetValue(themeName, out var theme) ? theme : AvailableThemes["Default"];
		});
		services.AddSingleton<SpectreUI>();
		services.AddSingleton<FileConcatenationService>();
		services.AddSingleton<Controller>();
	}
}
