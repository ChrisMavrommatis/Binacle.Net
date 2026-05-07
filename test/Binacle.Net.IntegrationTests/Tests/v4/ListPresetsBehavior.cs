using System.Net;
using System.Net.Http.Json;
using Binacle.Net.v4.Contracts.Presets;

namespace Binacle.Net.IntegrationTests.v4;
// TODO: Review
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class ListPresetsBehavior
{
	private const string routePath = "/api/v4/presets";

	private readonly BinacleApi sut;
	private readonly BinacleApiWithoutPresets sutWithoutPresets;

	public ListPresetsBehavior(BinacleApi sut, BinacleApiWithoutPresets sutWithoutPresets)
	{
		this.sut = sut;
		this.sutWithoutPresets = sutWithoutPresets;
	}

	[Fact(DisplayName = $"GET {routePath}. With Presets Configured, Returns 200 OK")]
	public async Task Get_WithPresetsConfigured_Returns_200Ok()
	{
		var response = await this.sut.Client.GetAsync(routePath, TestContext.Current.CancellationToken);
		response.StatusCode.ShouldBe(HttpStatusCode.OK);
	}

	[Fact(DisplayName = $"GET {routePath}. Without Presets Configured, Returns Empty Presets")]
	public async Task Get_WithoutPresetsConfigured_Returns_EmptyPresets()
	{
		var response = await this.sutWithoutPresets.Client.GetAsync(routePath, TestContext.Current.CancellationToken);
		response.StatusCode.ShouldBe(HttpStatusCode.OK);

		var presetsResponse = await response.Content.ReadFromJsonAsync<PresetListResponse>(
			this.sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		presetsResponse.ShouldNotBeNull();
		presetsResponse!.Presets.ShouldNotBeNull();
		presetsResponse!.Presets.ShouldBeEmpty();
	}
}
