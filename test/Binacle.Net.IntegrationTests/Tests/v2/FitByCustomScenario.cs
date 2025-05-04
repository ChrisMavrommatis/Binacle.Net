using System.Net;
using System.Net.Http.Json;
using Binacle.Net.TestsKernel.Data.Providers.BinaryDecision;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.v2.Models;
using Binacle.Net.v2.Requests;
using Binacle.Net.v2.Responses;

namespace Binacle.Net.IntegrationTests.v2;

[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class FitByCustomScenario
{
	private readonly BinacleApi sut;

	public FitByCustomScenario(BinacleApi sut)
	{
		this.sut = sut;
	}

	private const string routePath = "/api/v2/fit/by-custom";

	[Theory]
	[ClassData(typeof(BaselineScenarioTestDataProvider))]
	public Task BinaryDecision_Baseline(Scenario scenario)
		=> RunBinaryDecisionScenarioTest(scenario);

	[Theory]
	[ClassData(typeof(SimpleScenarioTestDataProvider))]
	public Task BinaryDecision_Simple(Scenario scenario)
		=> RunBinaryDecisionScenarioTest(scenario);

	[Theory]
	[ClassData(typeof(ComplexScenarioTestDataProvider))]
	public Task BinaryDecision_Complex(Scenario scenario)
		=> RunBinaryDecisionScenarioTest(scenario);

	private async Task RunBinaryDecisionScenarioTest(Scenario scenario)
	{
		var binCollection = scenario.GetBinCollectionKey();
		var expectedBin = scenario.GetTestBin(this.sut.BinCollectionsTestDataProvider);

		var request = new CustomFitRequest
		{
			Parameters = new()
			{
				FindSmallestBinOnly = false,
				ReportFittedItems = false,
				ReportUnfittedItems = false,
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

		
		var fitResponse = await response.Content.ReadFromJsonAsync<FitResponse>(
			this.sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		fitResponse.ShouldNotBeNull();
		fitResponse!.Data.ShouldHaveSingleItem();
		var result = fitResponse.Data.FirstOrDefault(x => x.Bin.ID == expectedBin.ID);
		result.ShouldNotBeNull();
		result!.Bin.ShouldNotBeNull();
		result.Bin.ID.ShouldBe(expectedBin.ID);

		var scenarioResult = scenario.ResultAs<BinaryDecisionScenarioResult>();

		if (scenarioResult.Fits)
		{
			fitResponse!.Result.ShouldBe(ResultType.Success);
			result.Result.ShouldBe(BinFitResultStatus.AllItemsFit);
		}
		else
		{
			fitResponse!.Result.ShouldBe(ResultType.Failure);
			result.Result.ShouldNotBe(BinFitResultStatus.AllItemsFit);
		}
	}
}

