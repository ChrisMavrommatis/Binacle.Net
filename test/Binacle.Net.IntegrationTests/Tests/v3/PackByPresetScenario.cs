using System.Net;
using System.Net.Http.Json;
using Binacle.Net.Configuration;
using Binacle.Net.v3.Contracts;
using Binacle.TestsKernel;
using Binacle.Lib;
using Binacle.TestsKernel.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.IntegrationTests.v3;

[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class PackByPresetScenario
{
	private readonly BinacleApi sut;

	public PackByPresetScenario(BinacleApi sut)
	{
		this.sut = sut;
	}

	private const string routePath = "/api/v3/pack/by-preset/{preset}";

	[Theory]
	[ClassData(typeof(CustomProblemsScenarioNameProvider))]
	public Task Custom_Problems(string scenario)
		=> RunTest(scenario);

	private async Task RunTest(string scenarioName)
	{
		var scenario = AllScenariosRegistry.GetScenarioByName(scenarioName);
		
		var presets = this.sut.Services.GetService<IOptions<BinPresetOptions>>();

		var preset = presets!.Value.Presets[PresetKeys.CustomProblems];
		var urlPath = routePath.Replace("{preset}", PresetKeys.CustomProblems);

		var request = new PackByPresetRequest()
		{
			Parameters = new()
			{
				Algorithm = Binacle.Net.Models.Algorithm.FFD,
				IncludeViPaqData = false
			},
			Items = scenario.Items.Select(x => new Box
			{
				ID = x.ID,
				Quantity = x.Quantity,
				Length = x.Length,
				Width = x.Width,
				Height = x.Height
			}).ToList()
		};

		var response = await this.sut.Client.PostAsJsonAsync(
			urlPath,
			request,
			this.sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		response.StatusCode.ShouldBe(HttpStatusCode.OK);

		var packResponse = await response.Content.ReadFromJsonAsync<PackResponse>(
			this.sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		packResponse.ShouldNotBeNull();
		packResponse!.Data.ShouldHaveCount(preset.Bins.Count);
		var result = packResponse.Data.FirstOrDefault(x => x.Bin.ID == scenario.Bin.ID);
		result.ShouldNotBeNull();
		result!.Bin.ShouldNotBeNull();
		result.Bin.ID.ShouldBe(scenario.Bin.ID);

		result.Bin.CalculateVolume().ShouldBe(scenario.Metrics.BinVolume);

		var itemsCount = (result.PackedItems?.Count ?? 0) 
		                 + (result.UnpackedItems?.Select(x => x.Quantity).Sum() ?? 0);
		itemsCount.ShouldBe(scenario.Metrics.ItemsCount);

		result.PackedBinVolumePercentage
			.ShouldBeLessThanOrEqualTo(scenario.Metrics.Percentage, new PercentageComparer());

		scenario.Result.EvaluateResult(result);
	}
	
}
