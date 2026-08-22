using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Binacle.Net.UIModule.IntegrationTests;

// The same image with the demo switched off. Every page route has to be gone, and nothing may start answering
// with a web page.
public sealed class UIModuleOffBinacleApi : WebApplicationFactory<IApiMarker>
{
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		var preBuildConfigurationValues = new Dictionary<string, string?>
		{
			{ "Features:UI_MODULE", bool.FalseString }
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
