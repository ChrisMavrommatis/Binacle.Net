using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Lib.Benchmarks.Providers;
using Binacle.TestsKernel;
using Binacle.TestsKernel.Models;

namespace Binacle.Lib.Benchmarks.Abstractions;

public abstract class TippingPointBenchmarkBase
{
	[Params(1, 2, 3, 4, 5, 6, 7)]
	public int BinCount { get; set; }

	[Params(4, 8, 12, 16, 20, 24, 28, 32, 36, 40, 44, 48, 52, 56, 60)]
	public int ItemCount { get; set; }

	[ParamsSource(typeof(ConcurrencyProvider), nameof(ConcurrencyProvider.GetProcessorCount))]
	public int ProcessorCount { get; set; }

	public List<TestBin> Bins { get; set; }
	public List<TestItem> Items { get; set; }
	
	[GlobalSetup]
	public void GlobalSetup()
	{
		var generator = new Generator(596333);
		this.Bins = generator.GenerateBins(this.BinCount, 100, 100, 100);
		this.Items = generator.GenerateItems(this.ItemCount, 5,25);
	}

	[GlobalCleanup]
	public void GlobalCleanup()
	{
	}
	
	protected abstract TestAlgorithmFactory<IPackingAlgorithm> AlgorithmFactory { get; }
	protected abstract AlgorithmOperation AlgorithmOperation { get; }
	
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public IDictionary<string, OperationResult> Loop()
	{
		var results = new Dictionary<string, OperationResult>(this.Bins.Count);

		for (var i = 0; i < this.Bins.Count; i++)
		{
			var bin = this.Bins[i];
			var algorithmInstance = this.AlgorithmFactory(bin, this.Items);
			var result = algorithmInstance.Execute(new TestOperationParameters()
			{
				Operation = this.AlgorithmOperation
			});
			results[bin.ID] = result;
		}

		return results;
	}

	[Benchmark]
	[BenchmarkOrder(20)]
	public IDictionary<string, OperationResult> ParallelConcurrent()
	{
		var results = new ConcurrentDictionary<string, OperationResult>(this.ProcessorCount, this.Bins.Count);

		Parallel.For(0, this.Bins.Count, i =>
		{
			var bin = this.Bins[i];
			var algorithmInstance = this.AlgorithmFactory(bin, this.Items);
			var result = algorithmInstance.Execute(new TestOperationParameters()
			{
				Operation = this.AlgorithmOperation
			});
			results[bin.ID] = result;
		});

		return results;
	}

	[Benchmark]
	[BenchmarkOrder(30)]
	public IDictionary<string, OperationResult> ParallelLock()
	{
		var results = new Dictionary<string, OperationResult>(this.Bins.Count);
		var resultsLock = new object();

		Parallel.For(0, this.Bins.Count, i =>
		{
			var bin = this.Bins[i];
			var algorithmInstance = this.AlgorithmFactory(bin, this.Items);
			var result = algorithmInstance.Execute(new TestOperationParameters()
			{
				Operation = this.AlgorithmOperation
			});
			lock (resultsLock)
			{
				results[bin.ID] = result;
			}
		});

		return results;
	}
}
