using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Api.IntegrationTests.Data.Providers.BinaryDecision;

internal class BaselineScenarioTestDataProvider : ScenarioTestDataProvider
{
	public BaselineScenarioTestDataProvider() : base(Constants.SolutionRootBasePath, "BinaryDecision/Baseline")
	{

	}
}
