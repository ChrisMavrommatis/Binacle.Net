namespace Binacle.Net.TestsKernel.Data.Providers.PackingEfficiency;


public sealed class OrLibraryScenarioDataProvider : MultipleCollectionScenarioDataProvider
{
	public OrLibraryScenarioDataProvider() 
		: base([
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
