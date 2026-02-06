using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.TestsKernel;
using Binacle.TestsKernel.Models;
using Binacle.TestsKernel.Providers;

namespace Binacle.Lib.Benchmarks.Abstractions;

public abstract class FastValidatonBenchmarkBase
{
	[ParamsSource(typeof(Providers.BenchmarkScenariosProvider), nameof(Providers.BenchmarkScenariosProvider.RepresentativeBaselineScenarios))]
	public string? Description { get; set; }
	
	public Scenario? Scenario { get; set; }
	
	[GlobalSetup]
	public void GlobalSetup()
	{
		var scenarioName = Providers.BenchmarkScenariosProvider.ScenarioDescriptions[this.Description!];
		this.Scenario = BischoffSuiteScenarioRegistry.GetScenarioByName(scenarioName);
	}
	
	[GlobalCleanup]
	public void GlobalCleanup()
	{
	}
	protected OperationResult Run(TestAlgorithmFactory<IPackingAlgorithm> algorithmFactory, AlgorithmOperation operation)
	{
		var algorithmInstance = algorithmFactory(this.Scenario!.Bin, this.Scenario!.Items);
		var result = algorithmInstance.Execute(new TestOperationParameters()
		{
			Operation = operation
		});
		return result;
	}
}
