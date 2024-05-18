using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Lib.UnitTests.Data.Providers;

internal class BaselineScenarioTestDataProvider : ScenarioTestDataProvider
{
	public BaselineScenarioTestDataProvider() : base(Constants.SolutionRootBasePath, "Baseline")
	{

	}
}
