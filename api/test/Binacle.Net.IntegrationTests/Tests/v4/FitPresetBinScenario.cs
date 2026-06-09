using System.Net;
using System.Net.Http.Json;
using Binacle.Lib;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Fit;
using Binacle.TestsKernel;
using Binacle.TestsKernel.Algorithms.Providers;

namespace Binacle.Net.IntegrationTests.v4;

[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class FitPresetBinScenario
{
	private const string routePath = "/api/v4/fit/bin/{preset}/{bin}";

	private readonly BinacleApi sut;

	public FitPresetBinScenario(BinacleApi sut)
	{
		this.sut = sut;
	}

	[Theory]
	[MemberData(nameof(CustomProblemsScenarioProvider.ScenarioNames), MemberType = typeof(CustomProblemsScenarioProvider))]
	public Task Custom_Problems(string scenario) => RunTest(scenario);

	private async Task RunTest(string scenarioName)
	{
		var scenario = AllScenariosProvider.GetScenarioByName(scenarioName);
		var url = routePath
			.Replace("{preset}", PresetKeys.CustomProblems)
			.Replace("{bin}", scenario.Bin.ID);

		var request = new FitPresetBinRequest
		{
			Parameters = new() { Algorithm = Binacle.Net.v4.Contracts.Algorithm.FFD },
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
			url,
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
