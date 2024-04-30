using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;
using Xunit;

namespace Binacle.Net.Api.IntegrationTests;

public class BinacleApiFactory : WebApplicationFactory<Binacle.Net.Api.IApiMarker>
{
	public BinacleApiFactory()
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
		});
	}

	public HttpClient Client { get; init; }
	public JsonSerializerOptions JsonSerializerOptions { get; init; }
}

[CollectionDefinition(Name)]
public class BinacleApiCollection : ICollectionFixture<BinacleApiFactory>
{
	public const string Name = "Binacle Api Collection";
	// This class has no code, and is never created. Its purpose is simply
	// to be the place to apply [CollectionDefinition] and all the
	// ICollectionFixture<> interfaces.
}
