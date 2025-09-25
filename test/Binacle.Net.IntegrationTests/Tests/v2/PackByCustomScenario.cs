using System.Net;
using System.Net.Http.Json;
using Binacle.Net.TestsKernel.Data.Providers.BinaryDecision;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.v2.Models;
using Binacle.Net.v2.Requests;
using Binacle.Net.v2.Responses;

namespace Binacle.Net.IntegrationTests.v2;

[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class PackByCustomScenario
{
	private readonly BinacleApi sut;

	public PackByCustomScenario(BinacleApi sut)
	{
		this.sut = sut;
	}

	private const string routePath = "/api/v2/pack/by-custom";

	[Theory]
	[ClassData(typeof(BaselineScenarioDataProvider))]
	public Task BinaryDecision_Baseline(Scenario scenario)
		=> RunBinaryDecisionScenarioTest(scenario);

	[Theory]
	[ClassData(typeof(SimpleScenarioDataProvider))]
	public Task BinaryDecision_Simple(Scenario scenario)
		=> RunBinaryDecisionScenarioTest(scenario);

	[Theory]
	[ClassData(typeof(ComplexScenarioDataProvider))]
	public Task BinaryDecision_Complex(Scenario scenario)
		=> RunBinaryDecisionScenarioTest(scenario);

	private async Task RunBinaryDecisionScenarioTest(Scenario scenario)
	{
		var binCollection = scenario.GetBinCollectionKey();
		var expectedBin = scenario.GetTestBin(this.sut.BinCollectionsDataProvider);

		var request = new CustomPackRequest
		{
			Parameters = new()
			{
				NeverReportUnpackedItems = false,
				StopAtSmallestBin = false,
				OptInToEarlyFails = false,
				ReportPackedItemsOnlyWhenFullyPacked = false
			},
			Bins = [
				new Bin
				{
					ID = expectedBin.ID,
					Length = expectedBin.Length,
					Width = expectedBin.Width,
					Height = expectedBin.Height
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
		packResponse!.Data.ShouldHaveCount(1);
		var result = packResponse.Data.FirstOrDefault(x => x.Bin.ID == expectedBin.ID);
		result.ShouldNotBeNull();
		result!.Bin.ShouldNotBeNull();
		result.Bin.ID.ShouldBe(expectedBin.ID);

		var scenarioResult = scenario.ResultAs<BinaryDecisionScenarioResult>();

		if (scenarioResult.Fits)
		{
			packResponse!.Result.ShouldBe(ResultType.Success);
			result.Result.ShouldBe(BinPackResultStatus.FullyPacked);
		}
		else
		{
			packResponse!.Result.ShouldBe(ResultType.Failure);
			result.Result.ShouldNotBe(BinPackResultStatus.FullyPacked);
		}
	}
}

