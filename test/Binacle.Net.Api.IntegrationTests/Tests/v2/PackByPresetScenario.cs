using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.TestsKernel.Data.Providers.BinaryDecision;
using Binacle.Net.TestsKernel.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Binacle.Net.Api.IntegrationTests.v2;

[Collection(BinacleApiCollection.Name)]
[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class PackByPresetScenario
{
	private readonly BinacleApiFactory sut;

	public PackByPresetScenario(BinacleApiFactory sut)
	{
		this.sut = sut;
	}

	private const string routePath = "/api/v2/pack/by-preset/{preset}";

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
		var expectedBin = scenario.GetTestBin(sut.BinCollectionsTestDataProvider);
		var preset = presets!.Value.Presets[binCollection];
		
		var urlPath = routePath.Replace("{preset}", binCollection);

		var request = new Api.v2.Requests.PresetPackRequest
		{
			Parameters = new()
			{
				NeverReportUnpackedItems = false,
				StopAtSmallestBin = false,
				OptInToEarlyFails = false,
				ReportPackedItemsOnlyWhenFullyPacked = false
			},
			Items = scenario.Items.Select(x => new Api.v2.Models.Box
			{
				ID = x.ID,
				Quantity = x.Quantity,
				Length = x.Length,
				Width = x.Width,
				Height = x.Height
			}).ToList()
		};

		var response = await this.sut.Client.PostAsJsonAsync(urlPath, request, this.sut.JsonSerializerOptions);

		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

		var packResponse = await response.Content.ReadFromJsonAsync<Api.v2.Responses.PackResponse>(this.sut.JsonSerializerOptions);

		packResponse.ShouldNotBeNull();
		packResponse!.Data.ShouldHaveCount(preset.Bins.Count);
		var result = packResponse.Data.FirstOrDefault(x => x.Bin.ID == expectedBin.ID);
		result.ShouldNotBeNull();
		result!.Bin.ShouldNotBeNull();
		result.Bin.ID.ShouldBe(expectedBin.ID);

		var scenarioResult = scenario.ResultAs<BinaryDecisionScenarioResult>();

		if (scenarioResult.Fits)
		{
			// Can't guarantee it as multiple bins are evaluated
			// fitResponse!.Result.Should().Be(Api.v2.Models.ResultType.Success);
			result.Result.ShouldBe(Api.v2.Models.BinPackResultStatus.FullyPacked);
		}
		else
		{
			// Can't guarantee it as multiple bins are evaluated
			// fitResponse!.Result.Should().Be(Api.v2.Models.ResultType.Failure);
			result.Result.ShouldNotBe(Api.v2.Models.BinPackResultStatus.FullyPacked);
		}
	}
}
