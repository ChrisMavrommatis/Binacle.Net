using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.TestsKernel.Data.Providers.BinaryDecision;
using Binacle.Net.TestsKernel.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Xunit;

namespace Binacle.Net.Api.IntegrationTests.v2;

[Collection(BinacleApiCollection.Name)]
[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class PackByCustomScenario
{
	private readonly BinacleApiFactory sut;

	public PackByCustomScenario(BinacleApiFactory sut)
	{
		this.sut = sut;
	}

	private const string routePath = "/api/v2/pack/by-custom";

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

		presets!.Value.Presets.Should().ContainKey(binCollection);

		var binPresetOption = presets.Value.Presets[binCollection];

		var request = new Api.v2.Requests.CustomPackRequest
		{
			Parameters = new()
			{
				NeverReportUnpackedItems = false,
				StopAtSmallestBin = false,
				OptInToEarlyFails = false,
				ReportPackedItemsOnlyWhenFullyPacked = false
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

		response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

		var packResponse = await response.Content.ReadFromJsonAsync<Api.v2.Responses.PackResponse>(this.sut.JsonSerializerOptions);

		packResponse.Should().NotBeNull();
		packResponse!.Data.Should()
			.NotBeNull()
			.And.NotBeEmpty()
			.And.HaveCount(1);
		var result = packResponse.Data.FirstOrDefault(x => x.Bin.ID == expectedBin.ID);
		result.Should().NotBeNull();
		result!.Bin.Should().NotBeNull();
		result.Bin.ID.Should().Be(expectedBin.ID);

		var scenarioResult = scenario.ResultAs<BinaryDecisionScenarioResult>();

		if (scenarioResult.Fits)
		{
			packResponse!.Result.Should().Be(Api.v2.Models.ResultType.Success);
			result.Result.Should().Be(Api.v2.Models.BinPackResultStatus.FullyPacked);
		}
		else
		{
			packResponse!.Result.Should().Be(Api.v2.Models.ResultType.Failure);
			result.Result.Should().NotBe(Api.v2.Models.BinPackResultStatus.FullyPacked);
		}
	}
}

