using Binacle.TestsKernel.Models;

namespace Binacle.Net.IntegrationTests.v3;

internal static class ScenarioResultExtensions
{
	public static void EvaluateResult(this ScenarioResult expected, Binacle.Net.v3.Contracts.BinFitResult actual)
	{
		var expectedStatus = Binacle.Net.v3.Contracts.FitResponse.MapResultStatus(expected.FittingStatus);
		actual.Result.ShouldBe(expectedStatus);
	}
	
	public static void EvaluateResult(this ScenarioResult expected, Binacle.Net.v3.Contracts.BinPackResult actual)
	{
		var expectedStatus = Binacle.Net.v3.Contracts.PackResponse.MapResultStatus(expected.PackingStatus);
		
		actual.Result.ShouldBe(expectedStatus);
	}
}
