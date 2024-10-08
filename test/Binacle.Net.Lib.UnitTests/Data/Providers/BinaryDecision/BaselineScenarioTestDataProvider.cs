using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Lib.UnitTests.Data.Providers.BinaryDecision;

internal class BaselineScenarioTestDataProvider : ScenarioTestDataProvider
{
	public BaselineScenarioTestDataProvider() : base(Constants.SolutionRootBasePath, "BinaryDecision/Baseline")
	{

	}
}
