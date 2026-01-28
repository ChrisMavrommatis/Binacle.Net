namespace Binacle.TestsKernel.Providers;

internal class CustomProblemsDataProvider : MultipleScenarioCollectionsDataProvider
{
	public CustomProblemsDataProvider()
		: base([
				"CustomProblems/baseline",
				"CustomProblems/simple",
				"CustomProblems/complex",
			]
		)
	{
	}
}
