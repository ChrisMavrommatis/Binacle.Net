using System.Net;
using System.Net.Http.Json;
using Binacle.Net.v3.Contracts;
using Binacle.TestsKernel;
using Binacle.Lib;
using Binacle.TestsKernel.Providers;

namespace Binacle.Net.IntegrationTests.v3;

[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class FitByCustomScenario
{
	private readonly BinacleApi sut;

	public FitByCustomScenario(BinacleApi sut)
	{
		this.sut = sut;
	}

	private const string routePath = "/api/v3/fit/by-custom";

	[Theory]
	[ClassData(typeof(CustomProblemsScenarioNameProvider))]
	public Task Custom_Problems(string scenario)
		=> RunTest(scenario);

	private async Task RunTest(string scenarioName)
	{
		var scenario = AllScenariosRegistry.GetScenarioByName(scenarioName);
		var request = new FitByCustomRequest
		{
			Parameters = new()
			{
				Algorithm = Binacle.Net.Models.Algorithm.FFD
			},
			Bins = [
				new Bin
				{
					ID = scenario.Bin.ID,
					Length = scenario.Bin.Length,
					Width = scenario.Bin.Width,
					Height = scenario.Bin.Height
				}
			],
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
			routePath,
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
		fitResponse!.Data.ShouldHaveSingleItem();
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

