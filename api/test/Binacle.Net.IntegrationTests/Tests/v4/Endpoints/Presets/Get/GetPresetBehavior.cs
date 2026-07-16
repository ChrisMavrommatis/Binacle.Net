using System.Net;
using System.Net.Http.Json;
using Binacle.Net.v4.Contracts.Presets;
using Binacle.TestsKernel.Algorithms.Providers;

namespace Binacle.Net.IntegrationTests.v4.Endpoints.Presets.Get;

// No scenario test for this one, and none for ListPresets either: the endpoint runs no algorithm, so there is
// nothing for a scenario to assert.
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class GetPresetBehavior
{
	private const string routePath = "/api/v4/presets/{preset}";

	private static string UrlFor(string preset) => routePath.Replace("{preset}", preset);

	private readonly BinacleApi sut;

	public GetPresetBehavior(BinacleApi sut)
	{
		this.sut = sut;
	}

	[Fact(DisplayName = $"GET {routePath}. With Existing Preset, Returns 200 OK")]
	public async Task Get_WithExistingPreset_Returns_200Ok()
	{
		var response = await this.sut.Client.GetAsync(
			UrlFor(PresetKeys.CustomProblems),
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.OK);
	}

	[Fact(DisplayName = $"GET {routePath}. With Non-Existing Preset, Returns 404 NotFound")]
	public async Task Get_WithNonExistingPreset_Returns_404NotFound()
	{
		var response = await this.sut.Client.GetAsync(
			UrlFor("non-existing-preset"),
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
	}

	[Fact(DisplayName = $"GET {routePath}. Returns The Requested Preset Name")]
	public async Task Get_ReturnsRequestedPresetName()
	{
		var preset = await GetPreset(PresetKeys.CustomProblems);
		preset.Name.ShouldBe(PresetKeys.CustomProblems);
	}

	[Fact(DisplayName = $"GET {routePath}. Returns The Bins Configured For The Preset")]
	public async Task Get_ReturnsConfiguredBins()
	{
		var preset = await GetPreset(PresetKeys.CustomProblems);
		preset.Bins.Select(x => x.ID).ShouldBe(CustomProblemsScenarioProvider.GetDistinctBinIds());
	}

	[Fact(DisplayName = $"GET {routePath}. Returns The Same Bins As The List Endpoint")]
	public async Task Get_AgreesWithListEndpoint()
	{
		var preset = await GetPreset(PresetKeys.SpecialSet);

		var listResponse = await this.sut.Client.GetFromJsonAsync<PresetListResponse>(
			"/api/v4/presets",
			this.sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		listResponse.ShouldNotBeNull();
		preset.Bins.Select(x => x.ID)
			.ShouldBe(listResponse!.Presets[PresetKeys.SpecialSet].Select(x => x.ID));
	}

	[Fact(DisplayName = $"GET {routePath}. Preset Bins Have Non-Zero Dimensions")]
	public async Task Get_PresetBins_HaveDimensions()
	{
		var preset = await GetPreset(PresetKeys.CustomProblems);
		foreach (var bin in preset.Bins)
		{
			bin.Length.ShouldBeGreaterThan(0);
			bin.Width.ShouldBeGreaterThan(0);
			bin.Height.ShouldBeGreaterThan(0);
		}
	}

	private async Task<PresetResponse> GetPreset(string preset)
	{
		var response = await this.sut.Client.GetAsync(UrlFor(preset), TestContext.Current.CancellationToken);
		response.StatusCode.ShouldBe(HttpStatusCode.OK);

		var presetResponse = await response.Content.ReadFromJsonAsync<PresetResponse>(
			this.sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		presetResponse.ShouldNotBeNull();
		return presetResponse!;
	}
}
