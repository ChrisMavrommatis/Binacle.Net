using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Models;
using Binacle.TestsKernel.ResultSelection.Models;
using Binacle.TestsKernel.ResultSelection.Providers;

namespace Binacle.Lib.Benchmarks.Abstractions;

public abstract class ResultSelectionBenchmarkBase
{
	public abstract string? ScenarioName { get; set; }
	public Scenario? Scenario { get; set; }
	
	[GlobalSetup]
	public void GlobalSetup()
	{
		this.Scenario = AllScenariosProvider.GetScenarioByName(this.ScenarioName!);
	}
	
	[GlobalCleanup]
	public void GlobalCleanup()
	{
	}
	protected OperationResult Run(IResultSelectionStrategy strategy)
	{
		var result = strategy.Select(this.Scenario!.Results);
		return result;
	}
}
