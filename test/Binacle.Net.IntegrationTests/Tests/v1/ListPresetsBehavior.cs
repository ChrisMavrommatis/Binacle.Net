using Binacle.Net.Configuration.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Binacle.Net.IntegrationTests.v1;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class ListPresetsBehavior
{
	private const string routePath = "/api/v1/presets";
	private readonly BinacleApiFactory sut;
	private readonly BinacleApiFactoryWithoutPresets sutWithoutPresets;

	public ListPresetsBehavior(
		BinacleApiFactory sut,
		BinacleApiFactoryWithoutPresets sutWithoutPresets
		)
	{
		this.sut = sut;
		this.sutWithoutPresets = sutWithoutPresets;
	}


	[Fact(DisplayName = $"GET {routePath}. With Presets Configured Returns 200 OK")]
	public async Task Get_WithPresetsConfigured_Returns_200Ok()
	{
		var response = await this.sut.Client.GetAsync(routePath, TestContext.Current.CancellationToken);

		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
	}

	[Fact(DisplayName = $"GET {routePath}. Without Presets Configured Returns 404 NotFound")]
	public async Task Get_WithoutPresetsConfigured_Returns_404NotFound()
	{
		var response = await this.sutWithoutPresets.Client.GetAsync(routePath, TestContext.Current.CancellationToken);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
	}

}
