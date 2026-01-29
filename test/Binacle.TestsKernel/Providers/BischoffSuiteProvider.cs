namespace Binacle.TestsKernel.Providers;

public static class CollectionKeys
{
	public static string[] BischoffSuite = [
		"BischoffSuite/orlib_thpack1",
		"BischoffSuite/orlib_thpack2",
		"BischoffSuite/orlib_thpack3",
		"BischoffSuite/orlib_thpack4",
		"BischoffSuite/orlib_thpack5",
		"BischoffSuite/orlib_thpack6",
		"BischoffSuite/orlib_thpack7",
	];
	
	public static string[] CustomProblems = [
		"CustomProblems/baseline",
		"CustomProblems/simple",
		"CustomProblems/complex",
	];
}

internal class BischoffSuiteProvider : MultipleScenarioCollectionsProvider
{
	public BischoffSuiteProvider()
		: base(CollectionKeys.BischoffSuite.ToArray())
	{
	}
}
