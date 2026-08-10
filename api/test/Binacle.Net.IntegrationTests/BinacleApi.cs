using System.Text.Json;
using System.Text.Json.Serialization;
using Binacle.Net.Configuration;
using Binacle.Net.IntegrationTests;
using Binacle.TestsKernel.Algorithms.Providers;
using Binacle.TestsKernel.Models;
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
			// Picks up the Test-environment config files the project ships alongside the app's own.
			.UseEnvironment("Test")
			// Seeds the builder: applied before Program.cs reaches WebApplication.CreateBuilder.
			.UseConfiguration(configuration)
			.ConfigureAppConfiguration(configurationBuilder =>
			{
				// Applied after the host built its own configuration, so these values win.
				configurationBuilder.AddInMemoryCollection(preBuildConfigurationValues);
			});

		builder.ConfigureTestServices(services =>
		{
			services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
			services.Configure<BinPresetOptions>(options =>
			{

				options.Presets.Clear();
				
				options.Presets.Add(PresetKeys.CustomProblems, new BinPresetOption()
				{
					Bins = ToBinOptions(CustomProblemsScenarioProvider.GetDistinctBins())
				});

				options.Presets.Add(PresetKeys.BiscoffSuite, new BinPresetOption()
				{
					Bins = ToBinOptions(BischoffSuiteScenarioProvider.GetDistinctBins())
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

	private static List<BinOption> ToBinOptions(IReadOnlyList<TestBin> bins)
		=> bins.Select(bin => new BinOption
		{
			ID = bin.ID,
			Length = bin.Length,
			Width = bin.Width,
			Height = bin.Height
		}).ToList();

	public HttpClient Client { get; init; }
	public JsonSerializerOptions JsonSerializerOptions { get; init; }
}
