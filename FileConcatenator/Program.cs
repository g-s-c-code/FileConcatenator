using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace FileConcatenator;

internal class Program
{
	private static readonly Theme DefaultTheme = new Theme(
			Color.White,
			Color.Grey,
			Color.SteelBlue1_1,
			Color.White,
			Color.Grey78
		);

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
		services.AddSingleton<Theme>(DefaultTheme);
		services.AddSingleton<SpectreUI>(provider =>
		{
			var theme = provider.GetRequiredService<Theme>();
			return new SpectreUI(theme);
		});
		services.AddSingleton<ConfigurationManager>();
		services.AddSingleton<FileConcatenationService>();
		services.AddSingleton<Controller>();
	}
}
