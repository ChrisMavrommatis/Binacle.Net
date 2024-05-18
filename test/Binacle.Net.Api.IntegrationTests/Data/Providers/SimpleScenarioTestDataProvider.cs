using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Api.IntegrationTests.Data.Providers;

internal class SimpleScenarioTestDataProvider : ScenarioTestDataProvider
{
	public SimpleScenarioTestDataProvider() : base(Constants.SolutionRootBasePath, "Simple")
	{

	}
}
