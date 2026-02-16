using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.TestsKernel;
using Binacle.TestsKernel.Models;
using Binacle.TestsKernel.ScenarioProviders;

namespace Binacle.Lib.Benchmarks.Abstractions;

public abstract class FastValidationBenchmarkBase
{
	[ParamsSource(typeof(Providers.BischoffCuratedProblemsProvider), nameof(Providers.BischoffCuratedProblemsProvider.RepresentativeBaselineScenarios))]
	public string? Description { get; set; }
	
	public Scenario? Scenario { get; set; }
	
	[GlobalSetup]
	public void GlobalSetup()
	{
		var scenarioName = Providers.BischoffCuratedProblemsProvider.ScenarioDescriptions[this.Description!];
		this.Scenario = BischoffSuiteScenarioProvider.GetScenarioByName(scenarioName);
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
