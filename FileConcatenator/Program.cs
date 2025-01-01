using Microsoft.Extensions.DependencyInjection;

namespace FileConcatenator;

internal class Program
{
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
		services.AddSingleton<ConcatenationService>();
		services.AddSingleton<SpectreUI>();
		services.AddSingleton<Controller>();
	}
}