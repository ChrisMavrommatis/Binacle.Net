using Binacle.Net.Configuration.Models;
using Binacle.Net.TestsKernel.Data.Providers.BinaryDecision;
using Binacle.Net.TestsKernel.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Binacle.Net.IntegrationTests.v1;

[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class QueryByCustomScenario
{
	private readonly BinacleApiFactory sut;

	public QueryByCustomScenario(BinacleApiFactory sut)
	{
		this.sut = sut;
	}

	private const string routePath = "/api/v1/query/by-custom";

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
		var presets = sut.Services.GetService<IOptions<BinPresetOptions>>();
		var binCollection = scenario.GetBinCollectionKey();
		var expectedBin = scenario.GetTestBin(this.sut.BinCollectionsTestDataProvider);

		presets!.Value.Presets.ShouldContainKey(binCollection);

		var binPresetOption = presets.Value.Presets[binCollection];

		var request = new Binacle.Net.v1.Requests.CustomQueryRequest
		{
			Bins = binPresetOption.Bins.Select(x => new Binacle.Net.v1.Models.Bin
			{
				ID = x.ID,
				Length = x.Length,
				Width = x.Width,
				Height = x.Height
			}).ToList(),
			Items = scenario.Items.Select(x => new Binacle.Net.v1.Models.Box
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

		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

		var result = await response.Content.ReadFromJsonAsync<Binacle.Net.v1.Responses.QueryResponse>(
			TestContext.Current.CancellationToken
		);

		result.ShouldNotBeNull();

		var scenarioResult = scenario.ResultAs<BinaryDecisionScenarioResult>();
		if (scenarioResult.Fits)
		{
			result!.Result.ShouldBe(Binacle.Net.v1.Models.ResultType.Success);
			result.Bin.ShouldNotBeNull();
			result.Bin!.ID.ShouldBe(expectedBin.ID);
		}
		else
		{
			result!.Result.ShouldBe(Binacle.Net.v1.Models.ResultType.Failure);
			result.Bin.ShouldBeNull();
		}
	}
}

