namespace FileConcatenator;

internal class Program
{
	private static void Main(string[] args)
	{
		IUserInterface ui = new ConsoleUI();
		var configService = new ConfigurationService();
		var fileConcatenationService = new FileConcatenationService(configService.LoadOrCreateConfig());
		var fileConcatenationController = new FileConcatenationController(ui, fileConcatenationService);
		var configurationController = new ConfigurationController(ui);
		var programController = new ProgramController(ui, configurationController, configService, fileConcatenationController, fileConcatenationService);

		programController.Run();
	}
}
