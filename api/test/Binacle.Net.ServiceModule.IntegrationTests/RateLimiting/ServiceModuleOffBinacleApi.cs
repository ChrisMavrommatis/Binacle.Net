using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Binacle.Net.ServiceModule.IntegrationTests.RateLimiting;

// The same host with the module off. It needs no database, no JWT settings and no rate limiter configuration -
// none of that is registered until AddServiceModule runs.
public sealed class ServiceModuleOffBinacleApi : WebApplicationFactory<IApiMarker>
{
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		var preBuildConfigurationValues = new Dictionary<string, string?>
		{
			{ "Features:SERVICE_MODULE", bool.FalseString }
		};

		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(preBuildConfigurationValues)
			.Build();

		builder
			.UseEnvironment("Test")
			.UseConfiguration(configuration)
			.ConfigureAppConfiguration(configurationBuilder =>
			{
				configurationBuilder.AddInMemoryCollection(preBuildConfigurationValues);
			});

		builder.ConfigureTestServices(services =>
		{
			services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
		});
	}
}
