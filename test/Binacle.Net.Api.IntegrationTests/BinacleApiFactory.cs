﻿using Binacle.Net.TestsKernel.Providers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Binacle.Net.Api.IntegrationTests;

public class BinacleApiFactory : WebApplicationFactory<Binacle.Net.Api.IApiMarker>
{
	public BinacleApiFactory()
	{
		this.Client = this.CreateClient();
		this.BinCollectionsTestDataProvider = new BinCollectionsTestDataProvider(Data.Constants.SolutionRootBasePath);

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
		var preBuildConfigurationValues = new Dictionary<string, string>
		{

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
		});
	}

	public HttpClient Client { get; init; }
	public BinCollectionsTestDataProvider BinCollectionsTestDataProvider { get; }
	public JsonSerializerOptions JsonSerializerOptions { get; init; }
}
