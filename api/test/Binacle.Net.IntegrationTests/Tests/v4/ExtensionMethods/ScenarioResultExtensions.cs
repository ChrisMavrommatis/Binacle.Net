using Binacle.Net.v4.Contracts.Fit;
using Binacle.Net.v4.Contracts.Pack;
using Binacle.TestsKernel.Algorithms.Models;

namespace Binacle.Net.IntegrationTests.v4.ExtensionMethods;

internal static class ScenarioResultExtensions
{
	public static void EvaluateResult(this ScenarioResult expected, FitBinResponse actual)
	{
		var expectedStatus = Binacle.Net.v4.ExtensionMethods.FittingMapperExtensions.MapToBinFitResultStatus(expected.FittingStatus);
		actual.Status.ShouldBe(expectedStatus);
		
		var expectedEarlyExitReason = Binacle.Net.v4.ExtensionMethods.FittingMapperExtensions.MapToBinFitEarlyExitReason(expected.FittingEarlyExitReason);
		actual.EarlyExitReason.ShouldBe(expectedEarlyExitReason);
	}

	public static void EvaluateResult(this ScenarioResult expected, PackBinResponse actual)
	{
		var expectedStatus = Binacle.Net.v4.ExtensionMethods.PackingMapperExtensions.MapToBinPackResultStatus(expected.PackingStatus);
		actual.Status.ShouldBe(expectedStatus);
	}
}
