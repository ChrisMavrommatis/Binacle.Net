using System.Net.Http.Json;
using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.TestsKernel.Data.Providers.BinaryDecision;
using Binacle.Net.TestsKernel.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.Api.IntegrationTests.v3;

[Collection(BinacleApiCollection.Name)]
[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class PackByCustomScenario
{
	private readonly BinacleApiFactory sut;

	public PackByCustomScenario(BinacleApiFactory sut)
	{
		this.sut = sut;
	}

	private const string routePath = "/api/v3/pack/by-custom";

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
		var presets = this.sut.Services.GetService<IOptions<BinPresetOptions>>();
		var binCollection = scenario.GetBinCollectionKey();
		var expectedBin = scenario.GetTestBin(this.sut.BinCollectionsTestDataProvider);

		presets!.Value.Presets.ShouldContainKey(binCollection);

		var binPresetOption = presets.Value.Presets[binCollection];

		var request = new Api.v3.Requests.CustomPackRequest
		{
			Parameters = new()
			{
				Algorithm = Models.Algorithm.FFD
			},
			Bins = [
				new Api.v3.Models.Bin
				{
					ID = expectedBin.ID,
					Length = expectedBin.Length,
					Width = expectedBin.Width,
					Height = expectedBin.Height
				}
			],
			Items = scenario.Items.Select(x => new Api.v3.Models.Box
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

		var packResponse = await response.Content.ReadFromJsonAsync<Api.v3.Responses.PackResponse>(this.sut.JsonSerializerOptions);

		packResponse.ShouldNotBeNull();
		packResponse!.Data.ShouldHaveSingleItem();
		var result = packResponse.Data.FirstOrDefault(x => x.Bin.ID == expectedBin.ID);
		result.ShouldNotBeNull();
		result!.Bin.ShouldNotBeNull();
		result.Bin.ID.ShouldBe(expectedBin.ID);

		var scenarioResult = scenario.ResultAs<BinaryDecisionScenarioResult>();

		if (scenarioResult.Fits)
		{
			packResponse!.Result.ShouldBe(Api.v3.Models.ResultType.Success);
			result.Result.ShouldBe(Api.v3.Models.BinPackResultStatus.FullyPacked);
		}
		else
		{
			packResponse!.Result.ShouldBe(Api.v3.Models.ResultType.Failure);
			result.Result.ShouldNotBe(Api.v3.Models.BinPackResultStatus.FullyPacked);
		}
	}
}
