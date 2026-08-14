using System.Net;
using System.Net.Http.Json;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Fit;
using Binacle.TestsKernel;
using Binacle.TestsKernel.Algorithms.Providers;
using Binacle.Net.IntegrationTests.v4.ExtensionMethods;

namespace Binacle.Net.IntegrationTests.v4.Endpoints.Fit.CustomBin;

[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class FitCustomBinScenario
{
	private const string routePath = "/api/v4/fit/bin";

	private readonly BinacleApi sut;

	public FitCustomBinScenario(BinacleApi sut)
	{
		this.sut = sut;
	}

	[Theory]
	[MemberData(nameof(CustomProblemsScenarioProvider.ScenarioNames), MemberType = typeof(CustomProblemsScenarioProvider))]
	public Task Custom_Problems(string scenario)
		=> RunTest(scenario);

	private async Task RunTest(string scenarioName)
	{
		var scenario = AllScenariosProvider.GetScenarioByName(scenarioName);

		var request = new FitCustomBinRequest
		{
			Parameters = new() { Algorithm = Binacle.Net.v4.Contracts.Algorithm.FFD },
			Bin = new()
			{
				ID = scenario.Bin.ID,
				Length = scenario.Bin.Length,
				Width = scenario.Bin.Width,
				Height = scenario.Bin.Height
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
			routePath,
			request,
			this.sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		response.StatusCode.ShouldBe(HttpStatusCode.OK);

		var result = await response.Content.ReadFromJsonAsync<FitBinResponse>(
			this.sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		result.ShouldNotBeNull();
		result!.Bin.ShouldNotBeNull();
		result.Bin.ID.ShouldBe(scenario.Bin.ID);
		result.Bin.CalculateVolume().ShouldBe(scenario.Metrics.BinVolume);

		var itemsCount = (result.PackedItems?.Count ?? 0)
		                 + (result.UnpackedItems?.Sum(x => x.Quantity) ?? 0);
		itemsCount.ShouldBe(scenario.Metrics.ItemsCount);

		result.PackedBinVolumePercentage
			.ShouldBeLessThanOrEqualTo(scenario.Metrics.Percentage, new PercentageComparer());

		scenario.Result.EvaluateResult(result);
	}
}
