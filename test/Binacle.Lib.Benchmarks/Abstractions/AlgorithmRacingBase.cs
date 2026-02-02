using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.TestsKernel;
using Binacle.TestsKernel.Models;
using Binacle.TestsKernel.Providers;

namespace Binacle.Lib.Benchmarks.Abstractions;

public abstract class AlgorithmRacingBase
{
	/*
	Scenario	BFD		FFD		WFD		Purpose
	thpack1_7	80.30%	78.08%	78.08%	Representative baseline
	thpack1_44	83.86%	62.65%	69.43%	BFD dominance (medium)
	thpack2_30	88.17%	87.75%	87.40%	High efficiency / low variance
	thpack2_35	85.86%	75.77%	56.82%	WFD weakness
	thpack7_56	84.65%	65.36%	60.74%	Hardest / max complexity
	*/
	
	[ParamsSource(typeof(Providers.BenchmarkScenariosProvider), nameof(Providers.BenchmarkScenariosProvider.GetBenchmarkScenarios))]
	public string? ScenarioName { get; set; }
	
	[ParamsSource(typeof(Providers.ConcurrencyProvider), nameof(Providers.ConcurrencyProvider.GetConcurrencyLevels))]
	public int ConcurrencyLevel { get; set; }
	public Scenario? Scenario { get; set; }
	
	[GlobalSetup]
	public void GlobalSetup()
	{
		this.Scenario = BischoffSuiteScenarioRegistry.GetScenarioByName(this.ScenarioName!);
	}
	
	[GlobalCleanup]
	public void GlobalCleanup()
	{
	}
	
	protected IDictionary<string,OperationResult> RunLoop(
		TestAlgorithmFactory<IPackingAlgorithm>[] algorithmFactories, 
		AlgorithmOperation operation
	)
	{
		var results = new Dictionary<string, OperationResult>(algorithmFactories.Length);

		for (var i = 0; i < algorithmFactories.Length; i++)
		{
			var algorithmFactory = algorithmFactories[i];
			var algorithmInstance = algorithmFactory(this.Scenario!.Bin, this.Scenario!.Items);
			var result = algorithmInstance.Execute(new TestOperationParameters()
			{
				Operation = operation
			});
			results[algorithmInstance.GetAlgorithmIdentifierName()] = result;
		}

		return results;
	}

	protected IDictionary<string, OperationResult> RunParallelConcurrent(
		TestAlgorithmFactory<IPackingAlgorithm>[] algorithmFactories,
		AlgorithmOperation operation
	)
	{
		var results =
			new ConcurrentDictionary<string, OperationResult>(this.ConcurrencyLevel, algorithmFactories.Length);

		Parallel.For(0, algorithmFactories.Length, i =>
		{
			var algorithmFactory = algorithmFactories[i];
			var algorithmInstance = algorithmFactory(this.Scenario!.Bin, this.Scenario!.Items);
			var result = algorithmInstance.Execute(new TestOperationParameters()
			{
				Operation = operation
			});
			results[algorithmInstance.GetAlgorithmIdentifierName()] = result;
		});

		return results;
	}
	
	protected IDictionary<string, OperationResult> RunParallelLock(
		TestAlgorithmFactory<IPackingAlgorithm>[] algorithmFactories,
		AlgorithmOperation operation
	)
	{
		var results = new Dictionary<string, OperationResult>(algorithmFactories.Length);
		var resultsLock = new object();

		Parallel.For(0, algorithmFactories.Length, i =>
		{
			var algorithmFactory = algorithmFactories[i];
			var algorithmInstance = algorithmFactory(this.Scenario!.Bin, this.Scenario!.Items);
			var result = algorithmInstance.Execute(new TestOperationParameters()
			{
				Operation = operation
			});
			lock (resultsLock)
			{
				results[algorithmInstance.GetAlgorithmIdentifierName()] = result;
			}
		});

		return results;
	}
	 
}
