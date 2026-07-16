using System.Net;
using System.Net.Http.Json;
using Binacle.Net.v4.Contracts.Presets;

namespace Binacle.Net.IntegrationTests.v4.Endpoints.Presets.List;

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

	[Fact(DisplayName = $"GET {routePath}. With Presets Configured, Returns All Configured Presets")]
	public async Task Get_WithPresetsConfigured_ReturnsAllPresets()
	{
		var presets = await GetPresets();
		presets.Presets.Count.ShouldBe(3);
		presets.Presets.ContainsKey(PresetKeys.CustomProblems).ShouldBeTrue();
		presets.Presets.ContainsKey(PresetKeys.BiscoffSuite).ShouldBeTrue();
		presets.Presets.ContainsKey(PresetKeys.SpecialSet).ShouldBeTrue();
	}

	[Fact(DisplayName = $"GET {routePath}. CustomProblems Preset Contains Expected Bins")]
	public async Task Get_CustomProblemsPreset_ContainsExpectedBins()
	{
		var presets = await GetPresets();
		var bins = presets.Presets[PresetKeys.CustomProblems].Select(b => b.ID).ToList();
		bins.ShouldContain("60x40x10");
		bins.ShouldContain("60x40x20");
		bins.ShouldContain("60x40x30");
	}

	[Fact(DisplayName = $"GET {routePath}. SpecialSet Preset Contains Expected Bins")]
	public async Task Get_SpecialSetPreset_ContainsExpectedBins()
	{
		var presets = await GetPresets();
		var bins = presets.Presets[PresetKeys.SpecialSet].Select(b => b.ID).ToList();
		bins.ShouldContain("special_bin_1");
		bins.ShouldContain("special_bin_2");
		bins.ShouldContain("special_bin_3");
	}

	[Fact(DisplayName = $"GET {routePath}. Preset Bins Have Non-Zero Dimensions")]
	public async Task Get_PresetBins_HaveDimensions()
	{
		var presets = await GetPresets();
		foreach (var preset in presets.Presets.Values)
		{
			foreach (var bin in preset)
			{
				bin.Length.ShouldBeGreaterThan(0);
				bin.Width.ShouldBeGreaterThan(0);
				bin.Height.ShouldBeGreaterThan(0);
			}
		}
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

	private async Task<PresetListResponse> GetPresets()
	{
		var response = await this.sut.Client.GetAsync(routePath, TestContext.Current.CancellationToken);
		response.StatusCode.ShouldBe(HttpStatusCode.OK);
		var presetsResponse = await response.Content.ReadFromJsonAsync<PresetListResponse>(
			this.sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);
		presetsResponse.ShouldNotBeNull();
		return presetsResponse!;
	}
}
