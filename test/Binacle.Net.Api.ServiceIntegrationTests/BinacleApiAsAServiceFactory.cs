using Binacle.Net.Api.ServiceIntegrationTests.MockServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;

namespace Binacle.Net.Api.ServiceIntegrationTests;

public class BinacleApiAsAServiceFactory : WebApplicationFactory<Binacle.Net.Api.IApiMarker>
{
	public BinacleApiAsAServiceFactory()
	{
		this.Client = this.CreateClient();

		this.JsonSerializerOptions = new()
		{
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		};
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		var preBuildConfigurationValues = new Dictionary<string, string>
		{
			{ "Features:SERVICE_MODULE", bool.TrueString },
		};
		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(preBuildConfigurationValues)
			.Build();

		// Additional configuration files are present when running in test
		// Because the project is setup to include the feature file, along with the environment as well
		// This will cause the tests to add the additional test config file
		builder.UseEnvironment("Test");

		// This configuration is used during the creation of the application
		// (e.g. BEFORE WebApplication.CreateBuilder(args) is called in Program.cs).
		builder.UseConfiguration(configuration);


		// This overrides configuration settings that were added as part
		// of building the Host (e.g. calling WebApplication.CreateBuilder(args)).
		builder.ConfigureAppConfiguration(configurationBuilder =>
		{

			//configurationBuilder.AddJsonFile();
		});


		builder.ConfigureTestServices(services =>
		{
			services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
			services.AddSingleton<FakeTelemetryChannel>();
		});

	}

	public HttpClient Client { get; init; }
	public JsonSerializerOptions JsonSerializerOptions { get; init; }
}
