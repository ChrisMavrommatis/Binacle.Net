using System.Net;
using System.Net.Http.Json;
using Binacle.Lib;
using Binacle.Net.v3.Contracts;
using Binacle.TestsKernel;
using Binacle.TestsKernel.Providers;
using Algorithm = Binacle.Net.Models.Algorithm;

namespace Binacle.Net.IntegrationTests.v3;

[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class PackByCustomScenario
{
	private readonly BinacleApi sut;

	public PackByCustomScenario(BinacleApi sut)
	{
		this.sut = sut;
	}

	private const string routePath = "/api/v3/pack/by-custom";

	[Theory]
	[ClassData(typeof(CustomProblemsScenarioNameProvider))]
	public Task Custom_Problems(string scenario)
		=> RunTest(scenario);

	private async Task RunTest(string scenarioName)
	{
		var scenario = AllScenariosRegistry.GetScenarioByName(scenarioName);
		
		var request = new PackByCustomRequest()
		{
			Parameters = new()
			{
				Algorithm = Algorithm.FFD,
				IncludeViPaqData = false
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

		var packResponse = await response.Content.ReadFromJsonAsync<PackResponse>(
			this.sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		packResponse.ShouldNotBeNull();
		packResponse!.Data.ShouldHaveSingleItem();
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

		// if (scenarioResult.Fits)
		// {
		// 	packResponse!.Result.ShouldBe(Binacle.Net.v3.Contracts.ResultType.Success);
		// 	result.Result.ShouldBe(Binacle.Net.v3.Contracts.BinPackResultStatus.FullyPacked);
		// }
		// else
		// {
		// 	packResponse!.Result.ShouldBe(Binacle.Net.v3.Contracts.ResultType.Failure);
		// 	result.Result.ShouldNotBe(Binacle.Net.v3.Contracts.BinPackResultStatus.FullyPacked);
		// }
	}
}
