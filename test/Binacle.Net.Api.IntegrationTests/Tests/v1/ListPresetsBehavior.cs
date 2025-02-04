using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.TestsKernel.TestPriority;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Binacle.Net.Api.IntegrationTests.v1;

[TestCaseOrderer("Binacle.Net.Api.Tests.TestPriority.TestPriorityOrderer", "Binacle.Net.Api.Tests")]
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class ListPresetsBehavior : IClassFixture<WebApplicationFactory<IApiMarker>>
{
	private const string routePath = "/api/v1/presets";
	private readonly WebApplicationFactory<IApiMarker> sut;
	private readonly HttpClient client;

	public ListPresetsBehavior(WebApplicationFactory<IApiMarker> sut)
	{
		this.sut = sut
			.WithWebHostBuilder(builder =>
			{
				builder.UseEnvironment("Test");
				builder.ConfigureTestServices(services =>
				{
					services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
				});
			});

		this.client = this.sut.CreateClient();
	}

	internal void Dispose()
	{
		this.client.Dispose();
		this.sut.Dispose();
	}

	[TestPriority(2)]
	[Fact(DisplayName = $"GET {routePath}. With Presets Configured Returns 200 OK")]
	public async Task Get_WithPresetsConfigured_Returns_200Ok()
	{
		var response = await this.client.GetAsync(routePath);

		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
	}

	[TestPriority(1)]
	[Fact(DisplayName = $"GET {routePath}. Without Presets Configured Returns 404 NotFound")]
	public async Task Get_WithoutPresetsConfigured_Returns_404NotFound()
	{
		// remove presets for this test
		var presetOptions = sut.Services.GetRequiredService<IOptions<BinPresetOptions>>();
		presetOptions.Value.Presets.Clear();

		var response = await this.client.GetAsync(routePath);

		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
	}

}
