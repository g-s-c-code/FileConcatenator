using System.Text;
using FileConcatenator.Models;
using FileConcatenator.Services;

namespace FileConcatenator.Tests
{
	[TestClass]
	public class ConfigurationServiceTests
	{
		private const string TestConfigPath = "test_config.json";
		private ConfigurationService _configService;

		[TestInitialize]
		public void Setup()
		{
			if (File.Exists(TestConfigPath))
			{
				File.Delete(TestConfigPath);
			}
			_configService = new ConfigurationService();
		}

		[TestMethod]
		public void LoadOrCreateConfig_WhenFileExists_LoadsExistingConfig()
		{
			// Arrange
			var initialConfig = new Configuration
			{
				BasePath = "/test/path",
				ShowHiddenFiles = true,
				FileTypes = new[] { "*.txt" },
				ClipboardCharacterLimit = 1000000
			};
			File.WriteAllText(TestConfigPath, Newtonsoft.Json.JsonConvert.SerializeObject(initialConfig));

			// Act
			_configService.LoadOrCreateConfig();

			// Assert
			Assert.IsFalse(_configService.IsNewConfig);
			Assert.IsNotNull(_configService.Config);
			Assert.AreEqual(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "\"/test/path\""), _configService.Config.BasePath);
			Assert.IsTrue(_configService.Config.ShowHiddenFiles);
			CollectionAssert.AreEqual(new[] { "*.txt" }, _configService.Config.FileTypes);
			Assert.AreEqual(1000000, _configService.Config.ClipboardCharacterLimit);
		}

		[TestMethod]
		public void SaveConfig_WritesConfigToFile()
		{
			// Arrange
			_configService.Config = new Configuration
			{
				BasePath = "/new/path",
				ShowHiddenFiles = false,
				FileTypes = new[] { "*.cs", "*.js" },
				ClipboardCharacterLimit = 2000000
			};

			// Act
			_configService.SaveConfig();

			// Assert
			Assert.IsTrue(File.Exists(TestConfigPath));
			var savedConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(TestConfigPath));
			Assert.AreEqual("/new/path", savedConfig.BasePath);
			Assert.IsFalse(savedConfig.ShowHiddenFiles);
			CollectionAssert.AreEqual(new[] { "*.cs", "*.js" }, savedConfig.FileTypes);
			Assert.AreEqual(2000000, savedConfig.ClipboardCharacterLimit);
		}
	}

	[TestClass]
	public class FileConcatenationServiceTests
	{
		private Configuration _config;
		private FileConcatenationService _service;

		[TestInitialize]
		public void Setup()
		{
			_config = new Configuration
			{
				BasePath = "/test/path",
				ShowHiddenFiles = false,
				FileTypes = new[] { "*.txt" },
				ClipboardCharacterLimit = 1000000
			};
			_service = new FileConcatenationService(_config);
		}

		[TestMethod]
		public void DisplayFileTypes_WritesCorrectOutput()
		{
			// Arrange
			var consoleOutput = new StringBuilder();
			Console.SetOut(new StringWriter(consoleOutput));

			// Act
			_service.DisplayFileTypes();

			// Assert
			StringAssert.Contains(consoleOutput.ToString(), "Currently Targeted File Types: *.txt");
		}
	}
}