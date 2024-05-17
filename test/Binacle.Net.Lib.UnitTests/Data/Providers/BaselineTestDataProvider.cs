using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Lib.UnitTests.Data.Providers;

internal class BaselineTestDataProvider : ScenarioTestDataProvider
{
	public BaselineTestDataProvider() : base(Constants.SolutionRootBasePath, "Baseline")
	{

	}
}
