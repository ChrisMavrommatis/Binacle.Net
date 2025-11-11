using System.Net;
using System.Net.Http.Json;
using Binacle.Net.Configuration;
using Binacle.Net.Models;
using Binacle.Net.TestsKernel.Data.Providers.BinaryDecision;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.v3.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.IntegrationTests.v3;

[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class FitByPresetScenario
{
	private readonly BinacleApi sut;

	public FitByPresetScenario(BinacleApi sut)
	{
		this.sut = sut;
	}

	private const string routePath = "/api/v3/fit/by-preset/{preset}";

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

		var request = new FitByPresetRequest
		{
			Parameters = new()
			{
				Algorithm = Algorithm.FFD
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

		var fitResponse = await response.Content.ReadFromJsonAsync<FitResponse>(
			this.sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		fitResponse.ShouldNotBeNull();
		fitResponse!.Data.ShouldHaveCount(preset.Bins.Count);
		var result = fitResponse.Data.FirstOrDefault(x => x.Bin.ID == expectedBin.ID);
		result.ShouldNotBeNull();
		result!.Bin.ShouldNotBeNull();
		result.Bin.ID.ShouldBe(expectedBin.ID);

		var scenarioResult = scenario.ResultAs<BinaryDecisionScenarioResult>();

		if (scenarioResult.Fits)
		{
			result.Result.ShouldBe(BinFitResultStatus.AllItemsFit);
		}
		else
		{
			result.Result.ShouldNotBe(BinFitResultStatus.AllItemsFit);
		}
	}
}
