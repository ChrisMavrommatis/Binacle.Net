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

    [Params(3, 7, 13, 17, 23, 29, 37, 47, 59, 67, 79)]
    public int ItemCount { get; set; }

    [ParamsSource(typeof(ConcurrencyProvider), nameof(ConcurrencyProvider.GetProcessorCount))]
    public int ProcessorCount { get; set; }

    public List<TestBin> Bins { get; set; } = null!;
    public List<TestItem> Items { get; set; } = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        this.Bins = SpecializedScalingProblemsProvider.GetBins(this.BinCount);
        this.Items = SpecializedScalingProblemsProvider.GetItems(this.ItemCount);
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
