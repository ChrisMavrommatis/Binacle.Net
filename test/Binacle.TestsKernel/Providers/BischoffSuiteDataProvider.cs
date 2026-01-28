namespace Binacle.TestsKernel.Providers;

internal class BischoffSuiteDataProvider : MultipleScenarioCollectionsDataProvider
{
	public BischoffSuiteDataProvider()
		: base([
				"BischoffSuite/orlib_thpack1",
				"BischoffSuite/orlib_thpack2",
				"BischoffSuite/orlib_thpack3",
				"BischoffSuite/orlib_thpack4",
				"BischoffSuite/orlib_thpack5",
				"BischoffSuite/orlib_thpack6",
				"BischoffSuite/orlib_thpack7",
			]
		)
	{
	}
}
