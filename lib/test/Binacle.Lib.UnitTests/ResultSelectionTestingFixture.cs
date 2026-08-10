using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Models;
using Binacle.TestsKernel.ResultSelection.Models;
using Binacle.TestsKernel.ResultSelection.Providers;

namespace Binacle.Lib.UnitTests;

public sealed class ResultSelectionTestingFixture : IDisposable
{

	public ResultSelectionTestingFixture()
	{

	}

	public void Dispose()
	{
	}

	// Arrange. The scenario holds the candidate results and the one the strategy is meant to pick.
	public Scenario GetScenarioByName(string scenarioName)
		=> AllScenariosProvider.GetScenarioByName(scenarioName);

	// Act. Selects, then projects to the string the test compares. No assertion here - the test makes its
	// own Shouldly call, which Sonar already recognises, so this side needs no [AssertionMethod] hint.
	public string Select(
		Scenario scenario,
		IResultSelectionStrategy selectionStrategy,
		Func<OperationResult, string> resultSelector
		)
	{
		var selected = selectionStrategy.Select(scenario.Results);

		return resultSelector(selected);
	}
}
