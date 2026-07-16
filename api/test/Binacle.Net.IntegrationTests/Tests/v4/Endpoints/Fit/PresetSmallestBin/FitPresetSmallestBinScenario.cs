using System.Net;
using System.Net.Http.Json;
using Binacle.Lib;
using Binacle.Lib.Abstractions.Models;
using Binacle.Net.v4.Contracts.Fit;
using Binacle.TestsKernel.Algorithms.Providers;

namespace Binacle.Net.IntegrationTests.v4.Endpoints.Fit.PresetSmallestBin;

// A scenario fixes the expected outcome for one bin, but this endpoint picks a bin across the whole preset, so
// the scenario's result cannot be asserted directly. What must hold is the selection invariant: the scenario's
// bin is one of the candidates, so if the items fit it, SmallestBin cannot return anything worse than a fit,
// nor a bin bigger than it.
[Trait("Scenario Tests", "Actual calculation for the algorithms")]
public class FitPresetSmallestBinScenario
{
	private const string routePath = "/api/v4/fit/smallest-bin/{preset}";

	private readonly BinacleApi sut;

	public FitPresetSmallestBinScenario(BinacleApi sut)
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
		var url = routePath.Replace("{preset}", PresetKeys.CustomProblems);

		var request = new FitPresetSmallestBinRequest
		{
			Parameters = new() { Algorithm = Binacle.Net.v4.Contracts.Algorithm.FFD },
			Items = scenario.Items.Select(x => new Binacle.Net.v4.Contracts.Box
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
		CustomProblemsScenarioProvider.GetDistinctBinIds().ShouldContain(result.Bin.ID);

		if (scenario.Result.FittingStatus == OperationResultStatus.FullyPacked)
		{
			result.Status.ShouldBe(BinFitResultStatus.Fits);
			result.Bin.CalculateVolume().ShouldBeLessThanOrEqualTo(scenario.Metrics.BinVolume);
		}
	}
}
