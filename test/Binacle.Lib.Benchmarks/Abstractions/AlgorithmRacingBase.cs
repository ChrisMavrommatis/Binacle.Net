using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Benchmarks.Order;
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
	
	protected abstract TestAlgorithmFactory<IPackingAlgorithm>[] Algorithms { get; }
	protected abstract AlgorithmOperation AlgorithmOperation { get; }
	
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public IDictionary<string,OperationResult> Loop()
	{
		var results = new Dictionary<string, OperationResult>(this.Algorithms.Length);

		for (var i = 0; i < this.Algorithms.Length; i++)
		{
			var algorithmFactory = this.Algorithms[i];
			var algorithmInstance = algorithmFactory(this.Scenario!.Bin, this.Scenario!.Items);
			var result = algorithmInstance.Execute(new TestOperationParameters()
			{
				Operation = this.AlgorithmOperation
			});
			results[algorithmInstance.GetAlgorithmIdentifierName()] = result;
		}

		return results;
	}

	[Benchmark]
	[BenchmarkOrder(20)]
	public IDictionary<string, OperationResult> ParallelConcurrent()
	{
		var results =
			new ConcurrentDictionary<string, OperationResult>(this.ProcessorCount, this.Algorithms.Length);

		Parallel.For(0, this.Algorithms.Length, i =>
		{
			var algorithmFactory = this.Algorithms[i];
			var algorithmInstance = algorithmFactory(this.Scenario!.Bin, this.Scenario!.Items);
			var result = algorithmInstance.Execute(new TestOperationParameters()
			{
				Operation = this.AlgorithmOperation
			});
			results[algorithmInstance.GetAlgorithmIdentifierName()] = result;
		});

		return results;
	}
	
	[Benchmark]
	[BenchmarkOrder(30)]
	public IDictionary<string, OperationResult> ParallelLock()
	{
		var results = new Dictionary<string, OperationResult>(this.Algorithms.Length);
		var resultsLock = new object();

		Parallel.For(0, this.Algorithms.Length, i =>
		{
			var algorithmFactory = this.Algorithms[i];
			var algorithmInstance = algorithmFactory(this.Scenario!.Bin, this.Scenario!.Items);
			var result = algorithmInstance.Execute(new TestOperationParameters()
			{
				Operation = this.AlgorithmOperation
			});
			lock (resultsLock)
			{
				results[algorithmInstance.GetAlgorithmIdentifierName()] = result;
			}
		});

		return results;
	}
	 
}
