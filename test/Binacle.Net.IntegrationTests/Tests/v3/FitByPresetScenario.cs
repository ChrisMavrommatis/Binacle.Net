using System.Net;
using System.Net.Http.Json;
using Binacle.Net.Configuration;
using Binacle.Net.v3.Contracts;
using Binacle.Lib;
using Binacle.TestsKernel;
using Binacle.TestsKernel.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace Binacle.Net.IntegrationTests.v3;

[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class FitByPresetScenario
{
	private readonly BinacleApi sut;

	public FitByPresetScenario(BinacleApi sut)
	{
		this.sut = sut;
	}

	private const string routePath = "/api/v3/fit/by-preset/{preset}";

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

		var request = new FitByPresetRequest
		{
			Parameters = new()
			{
				Algorithm = Binacle.Net.Models.Algorithm.FFD
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

		var fitResponse = await response.Content.ReadFromJsonAsync<FitResponse>(
			this.sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		fitResponse.ShouldNotBeNull();
		fitResponse!.Data.ShouldHaveCount(preset.Bins.Count);
		var result = fitResponse.Data.FirstOrDefault(x => x.Bin.ID == scenario.Bin.ID);
		result.ShouldNotBeNull();
		result!.Bin.ShouldNotBeNull();
		result.Bin.ID.ShouldBe(scenario.Bin.ID);

		result.Bin.CalculateVolume().ShouldBe(scenario.Metrics.BinVolume);

		var itemsCount = (result.FittedItems?.Count ?? 0) 
		                 + (result.UnfittedItems?.Select(x => x.Quantity).Sum() ?? 0);
		itemsCount.ShouldBe(scenario.Metrics.ItemsCount);

		result.FittedBinVolumePercentage!.Value
			.ShouldBeLessThanOrEqualTo(scenario.Metrics.Percentage, new PercentageComparer());
		
		scenario.Result.EvaluateResult(result);
	}
}
