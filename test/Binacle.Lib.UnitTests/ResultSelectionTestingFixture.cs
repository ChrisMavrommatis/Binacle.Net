using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Models;
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
	
	public void RunTest(
		string scenarioName, 
		IResultSelectionStrategy selectionStrategy,
		Func<OperationResult, string> resultSelector
		)
	{
		var scenario = AllScenariosProvider.GetScenarioByName(scenarioName);
		var selected = selectionStrategy.Select(scenario.Results);
		var selectedResult = resultSelector(selected);
		selectedResult.ShouldBe(scenario.ExpectedResult);
	}
}
