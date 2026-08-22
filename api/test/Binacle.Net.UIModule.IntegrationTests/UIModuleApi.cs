using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Binacle.Net.UIModule.IntegrationTests;

// A host with the demo switched one way or the other. Its own factory rather than the assembly fixture,
// which runs with every module off - the whole point here is what changes once a module serves web pages.
public abstract class UIModuleApi : WebApplicationFactory<IApiMarker>
{
	private readonly Dictionary<string, string?> featureValues;

	protected UIModuleApi(bool uiModuleEnabled)
	{
		this.featureValues = new Dictionary<string, string?>
		{
			{ "Features:UI_MODULE", uiModuleEnabled ? bool.TrueString : bool.FalseString }
		};
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		// Program.cs reads the flag while the host builds, so the values go in twice: once on the builder
		// itself, and once through the app configuration the built host ends up with.
		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(this.featureValues)
			.Build();

		builder
			.UseEnvironment("Test")
			.UseConfiguration(configuration)
			.ConfigureAppConfiguration(configurationBuilder =>
			{
				configurationBuilder.AddInMemoryCollection(this.featureValues);
			});

		builder.ConfigureTestServices(services =>
		{
			services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
		});
	}
}
