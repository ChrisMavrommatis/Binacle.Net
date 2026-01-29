namespace Binacle.TestsKernel.Providers;

internal class CustomProblemsProvider : MultipleScenarioCollectionsProvider
{
	public CustomProblemsProvider()
		: base([
				"CustomProblems/baseline",
				"CustomProblems/simple",
				"CustomProblems/complex",
			]
		)
	{
	}
}
