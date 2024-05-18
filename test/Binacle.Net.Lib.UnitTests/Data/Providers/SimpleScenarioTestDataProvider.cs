using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Lib.UnitTests.Data.Providers;

internal class SimpleScenarioTestDataProvider : ScenarioTestDataProvider
{
	public SimpleScenarioTestDataProvider() : base(Constants.SolutionRootBasePath, "Simple")
	{

	}
}
