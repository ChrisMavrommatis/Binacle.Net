using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.TestsKernel.Data.Providers.BinaryDecision;
using Binacle.Net.TestsKernel.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Binacle.Net.Api.IntegrationTests.v2;

[Collection(BinacleApiCollection.Name)]
[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class FitByCustomScenario
{
	private readonly BinacleApiFactory sut;

	public FitByCustomScenario(BinacleApiFactory sut)
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

		var request = new Api.v2.Requests.CustomFitRequest
		{
			Parameters = new()
			{
				FindSmallestBinOnly = false,
				ReportFittedItems = false,
				ReportUnfittedItems = false,
			},
			Bins = [
				new Api.v2.Models.Bin
				{
					ID = expectedBin.ID,
					Length = expectedBin.Length,
					Width = expectedBin.Width,
					Height = expectedBin.Height
				}
			],
			Items = scenario.Items.Select(x => new Api.v2.Models.Box
			{
				ID = x.ID,
				Quantity = x.Quantity,
				Length = x.Length,
				Width = x.Width,
				Height = x.Height
			}).ToList()
		};

		var response = await this.sut.Client.PostAsJsonAsync(routePath, request, this.sut.JsonSerializerOptions);

		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

		
		var fitResponse = await response.Content.ReadFromJsonAsync<Api.v2.Responses.FitResponse>(this.sut.JsonSerializerOptions);

		fitResponse.ShouldNotBeNull();
		fitResponse!.Data.ShouldHaveSingleItem();
		var result = fitResponse.Data.FirstOrDefault(x => x.Bin.ID == expectedBin.ID);
		result.ShouldNotBeNull();
		result!.Bin.ShouldNotBeNull();
		result.Bin.ID.ShouldBe(expectedBin.ID);

		var scenarioResult = scenario.ResultAs<BinaryDecisionScenarioResult>();

		if (scenarioResult.Fits)
		{
			fitResponse!.Result.ShouldBe(Api.v2.Models.ResultType.Success);
			result.Result.ShouldBe(Api.v2.Models.BinFitResultStatus.AllItemsFit);
		}
		else
		{
			fitResponse!.Result.ShouldBe(Api.v2.Models.ResultType.Failure);
			result.Result.ShouldNotBe(Api.v2.Models.BinFitResultStatus.AllItemsFit);
		}
	}
}

