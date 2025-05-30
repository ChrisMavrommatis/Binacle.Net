using System.Net;
using System.Net.Http.Json;
using Binacle.Net.Configuration.Models;
using Binacle.Net.TestsKernel.Data.Providers.BinaryDecision;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.v2.Models;
using Binacle.Net.v2.Requests;
using Binacle.Net.v2.Responses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.IntegrationTests.v2;

[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class PackByPresetScenario
{
	private readonly BinacleApi sut;

	public PackByPresetScenario(BinacleApi sut)
	{
		this.sut = sut;
	}

	private const string routePath = "/api/v2/pack/by-preset/{preset}";

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
		var presets = this.sut.Services.GetService<IOptions<BinPresetOptions>>();

		var binCollection = scenario.GetBinCollectionKey();
		var expectedBin = scenario.GetTestBin(sut.BinCollectionsDataProvider);
		var preset = presets!.Value.Presets[binCollection];
		
		var urlPath = routePath.Replace("{preset}", binCollection);

		var request = new PresetPackRequest
		{
			Parameters = new()
			{
				NeverReportUnpackedItems = false,
				StopAtSmallestBin = false,
				OptInToEarlyFails = false,
				ReportPackedItemsOnlyWhenFullyPacked = false
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

		var packResponse = await response.Content.ReadFromJsonAsync<PackResponse>(
			this.sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

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
			// fitResponse!.Result.Should().Be(v2.Models.ResultType.Success);
			result.Result.ShouldBe(BinPackResultStatus.FullyPacked);
		}
		else
		{
			// Can't guarantee it as multiple bins are evaluated
			// fitResponse!.Result.Should().Be(v2.Models.ResultType.Failure);
			result.Result.ShouldNotBe(BinPackResultStatus.FullyPacked);
		}
	}
}
