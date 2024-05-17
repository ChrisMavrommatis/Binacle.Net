using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Lib.UnitTests.Data.Providers;

internal class SimpleTestDataProvider : ScenarioTestDataProvider
{
	public SimpleTestDataProvider() : base(Constants.SolutionRootBasePath, "Simple")
	{

	}
}
