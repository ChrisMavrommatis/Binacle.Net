using System.Text.Json;
using System.Text.Json.Serialization;
using Binacle.Net.Configuration;
using Binacle.Net.IntegrationTests;
using Binacle.TestsKernel.Providers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

[assembly: AssemblyFixture(typeof(BinacleApi))]

namespace Binacle.Net.IntegrationTests;

public class BinacleApi : WebApplicationFactory<IApiMarker>
{
	
	public BinacleApi()
	{
		this.Client = this.CreateClient();
		this.JsonSerializerOptions = new()
		{
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			Converters = { new JsonStringEnumConverter() },
		};
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		// TODO: Run the tests with all modules enabled
		var preBuildConfigurationValues = new Dictionary<string, string?>
		{
		};
		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(preBuildConfigurationValues)
			.Build();

		builder
			// Additional configuration files are present when running in test
			// Because the project is set up to include the feature file, along with the environment as well
			// This will cause the tests to add the additional test config file
			.UseEnvironment("Test")
			// This configuration is used during the creation of the application
			// (e.g. BEFORE WebApplication.CreateBuilder(args) is called in Program.cs).
			.UseConfiguration(configuration)
			.ConfigureAppConfiguration(configurationBuilder =>
			{
				// This overrides configuration settings that were added as part
				// of building the Host (e.g. calling WebApplication.CreateBuilder(args)).
				configurationBuilder.AddInMemoryCollection(preBuildConfigurationValues);
				//configurationBuilder.AddJsonFile();
			});

		builder.ConfigureTestServices(services =>
		{
			services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
			services.Configure<BinPresetOptions>(options =>
			{

				options.Presets.Clear();
				
				var customProblemBins = CustomProblemsScenarioRegistry
					.GetScenarios()
					.Select(x => x.Bin)
					.DistinctBy(x => x.ID)
					.Select(x => new BinOption
					{
						ID = x.ID,
						Length = x.Length,
						Width = x.Width,
						Height = x.Height
					}).ToList();
				
				options.Presets.Add(PresetKeys.CustomProblems, new BinPresetOption()
				{
					Bins = customProblemBins
				});
				
				var bischoffSuiteBins = BischoffSuiteScenarioRegistry
					.GetScenarios()
					.Select(x => x.Bin)
					.DistinctBy(x => x.ID)
					.Select(x => new BinOption
					{
						ID = x.ID,
						Length = x.Length,
						Width = x.Width,
						Height = x.Height
					}).ToList();
				
				options.Presets.Add(PresetKeys.BiscoffSuite, new BinPresetOption()
				{
					Bins = bischoffSuiteBins
				});

				options.Presets.Add(PresetKeys.SpecialSet, new BinPresetOption()
				{
					Bins = [
						new BinOption
						{
							ID= "special_bin_1",
							Length= 60,
							Width= 40,
							Height= 10
						},
						new BinOption
						{
							ID= "special_bin_2",
							Length= 60,
							Width= 40,
							Height= 11
						},
						new BinOption
						{
							ID= "special_bin_3",
							Length= 60,
							Width= 40,
							Height= 12
						}
					]
				});
			});
		});
	}

	public HttpClient Client { get; init; }
	public JsonSerializerOptions JsonSerializerOptions { get; init; }
}
