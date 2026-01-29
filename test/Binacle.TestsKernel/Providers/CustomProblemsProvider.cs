namespace Binacle.TestsKernel.Providers;

internal class CustomProblemsProvider : MultipleScenarioCollectionsProvider
{
	public CustomProblemsProvider()
		: base(CollectionKeys.CustomProblems.ToArray())
	{
	}
}
