using System.Net;
using System.Net.Http.Json;
using Binacle.Net.TestsKernel.Data.Providers.BinaryDecision;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.v1.Models;
using Binacle.Net.v1.Requests;
using Binacle.Net.v1.Responses;

namespace Binacle.Net.IntegrationTests.v1;

[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class QueryByPresetScenario
{
	private readonly BinacleApi sut;

	public QueryByPresetScenario(BinacleApi sut)
	{
		this.sut = sut;
	}

	private const string routePath = "/api/v1/query/by-preset/{preset}";

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

		var urlPath = routePath.Replace("{preset}", binCollection);

		var request = new PresetQueryRequest
		{
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

		var result = await response.Content.ReadFromJsonAsync<QueryResponse>(
			TestContext.Current.CancellationToken
		);

		result.ShouldNotBeNull();

		var scenarioResult = scenario.ResultAs<BinaryDecisionScenarioResult>();

		if (scenarioResult.Fits)
		{
			result!.Result.ShouldBe(ResultType.Success);
			result.Bin.ShouldNotBeNull();
			result.Bin!.ID.ShouldBe(expectedBin.ID);
		}
		else
		{
			result!.Result.ShouldBe(ResultType.Failure);
			result.Bin.ShouldBeNull();
		}
	}
}

