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
		builder.UseEnvironment("Development");
		builder.ConfigureTestServices(services =>
		{
			services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
			services.AddSingleton<FakeTelemetryChannel>();
		});
	}

	// Need this to run before the program.cs runs
	protected override IHost CreateHost(IHostBuilder builder)
	{
		builder.ConfigureHostConfiguration(config =>
		{
			var configurationValues = new Dictionary<string, string?>
			{
				{ "Features:SERVICE_MODULE", bool.TrueString }
			};
			config.AddInMemoryCollection(configurationValues);
		});

		return base.CreateHost(builder);
	}
	public HttpClient Client { get; init; }
	public JsonSerializerOptions JsonSerializerOptions { get; init; }
}
