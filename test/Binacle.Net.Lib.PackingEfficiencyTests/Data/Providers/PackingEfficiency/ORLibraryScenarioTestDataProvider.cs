using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Lib.PackingEfficiencyTests.Data.Providers.PackingEfficiency;

internal class ORLibraryScenarioTestDataProvider : MultipleCollectionScenarioTestDataProvider
{
	public ORLibraryScenarioTestDataProvider() 
		: base(Constants.SolutionRootBasePath, [
			"PackingEfficiency/orlib_thpack1",
			"PackingEfficiency/orlib_thpack2",
			"PackingEfficiency/orlib_thpack3",
			"PackingEfficiency/orlib_thpack4",
			"PackingEfficiency/orlib_thpack5",
			"PackingEfficiency/orlib_thpack6",
			"PackingEfficiency/orlib_thpack7",
			]
		)
	{

	}
}
