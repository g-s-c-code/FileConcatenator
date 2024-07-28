﻿using Microsoft.Extensions.DependencyInjection;

namespace FileConcatenator
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var serviceCollection = new ServiceCollection();
			ConfigureServices(serviceCollection);
			var serviceProvider = serviceCollection.BuildServiceProvider();

			var programController = serviceProvider.GetService<ProgramController>();
			programController?.Run();
		}

		private static void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IUserInterface, ConsoleUI>();
			services.AddSingleton<ConfigurationService>();
			services.AddSingleton<FileConcatenationService>();
			services.AddSingleton<ConfigurationController>();
			services.AddSingleton<FileConcatenationController>();
			services.AddSingleton<ProgramController>();
		}
	}
}
