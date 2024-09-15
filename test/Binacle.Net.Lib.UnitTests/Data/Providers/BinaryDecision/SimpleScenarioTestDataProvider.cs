using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Lib.UnitTests.Data.Providers.BinaryDecision;

internal class SimpleScenarioTestDataProvider : ScenarioTestDataProvider
{
	public SimpleScenarioTestDataProvider() : base(Constants.SolutionRootBasePath, "BinaryDecision/Simple")
	{

	}
}
