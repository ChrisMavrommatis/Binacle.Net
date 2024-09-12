using Binacle.Net.TestsKernel.Providers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;

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
		};
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.UseEnvironment("Test");
		builder.ConfigureTestServices(services =>
		{
			services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
		});
	}

	public HttpClient Client { get; init; }
	public BinCollectionsTestDataProvider BinCollectionsTestDataProvider { get; }
	public JsonSerializerOptions JsonSerializerOptions { get; init; }
}
