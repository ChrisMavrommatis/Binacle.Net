using System.Net.Http.Json;
using Binacle.Net.Configuration.Models;
using Binacle.Net.Models;
using Binacle.Net.TestsKernel.Data.Providers.BinaryDecision;
using Binacle.Net.TestsKernel.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
		var expectedBin = scenario.GetTestBin(this.sut.BinCollectionsDataProvider);

		presets!.Value.Presets.ShouldContainKey(binCollection);

		var binPresetOption = presets.Value.Presets[binCollection];

		var request = new Binacle.Net.v3.Contracts.PackByCustomRequest()
		{
			Parameters = new()
			{
				Algorithm = Algorithm.FFD,
				IncludeViPaqData = false
			},
			Bins = [
				new Binacle.Net.v3.Contracts.Bin
				{
					ID = expectedBin.ID,
					Length = expectedBin.Length,
					Width = expectedBin.Width,
					Height = expectedBin.Height
				}
			],
			Items = scenario.Items.Select(x => new Binacle.Net.v3.Contracts.Box
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

		var packResponse = await response.Content.ReadFromJsonAsync<Binacle.Net.v3.Contracts.PackResponse>(
			this.sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		packResponse.ShouldNotBeNull();
		packResponse!.Data.ShouldHaveSingleItem();
		var result = packResponse.Data.FirstOrDefault(x => x.Bin.ID == expectedBin.ID);
		result.ShouldNotBeNull();
		result!.Bin.ShouldNotBeNull();
		result.Bin.ID.ShouldBe(expectedBin.ID);

		var scenarioResult = scenario.ResultAs<BinaryDecisionScenarioResult>();

		if (scenarioResult.Fits)
		{
			packResponse!.Result.ShouldBe(Binacle.Net.v3.Contracts.ResultType.Success);
			result.Result.ShouldBe(Binacle.Net.v3.Contracts.BinPackResultStatus.FullyPacked);
		}
		else
		{
			packResponse!.Result.ShouldBe(Binacle.Net.v3.Contracts.ResultType.Failure);
			result.Result.ShouldNotBe(Binacle.Net.v3.Contracts.BinPackResultStatus.FullyPacked);
		}
	}
}
