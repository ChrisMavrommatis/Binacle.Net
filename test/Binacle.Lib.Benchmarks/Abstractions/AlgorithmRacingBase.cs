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
	[ParamsSource(typeof(Providers.BenchmarkScenariosProvider), nameof(Providers.BenchmarkScenariosProvider.GetBenchmarkScenarios))]
	public string? Description { get; set; }
	
	[ParamsSource(typeof(Providers.ConcurrencyProvider), nameof(Providers.ConcurrencyProvider.GetProcessorCount))]
	public int ProcessorCount { get; set; }
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
			new ConcurrentDictionary<string, OperationResult>(this.ProcessorCount, algorithmFactories.Length);

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
