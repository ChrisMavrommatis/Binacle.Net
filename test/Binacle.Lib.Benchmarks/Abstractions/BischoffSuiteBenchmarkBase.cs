using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Benchmarks.Providers;
using Binacle.TestsKernel;
using Binacle.TestsKernel.Models;

namespace Binacle.Lib.Benchmarks.Abstractions;

public abstract class BischoffSuiteBenchmarkBase
{
	[ParamsSource(typeof(BischoffSuiteScenarioProvider), nameof(BischoffSuiteScenarioProvider.GetScenarioNames))]
	public string ScenarioName { get; set; }
	public Scenario Scenario { get; set; }
	
	[GlobalSetup]
	public void GlobalSetup()
	{
		this.Scenario = BischoffSuiteScenarioProvider.GetScenarioByName(this.ScenarioName);
	}
	
	[GlobalCleanup]
	public void GlobalCleanup()
	{
	}
	protected OperationResult Run(TestAlgorithmFactory<IPackingAlgorithm> algorithmFactory, AlgorithmOperation operation)
	{
		var algorithmInstance = algorithmFactory(this.Scenario.Bin, this.Scenario.Items);
		var result = algorithmInstance.Execute(new TestOperationParameters()
		{
			Operation = operation
		});
		return result;
	}
}
