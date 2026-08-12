using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Lib.Benchmarks.Providers;
using Binacle.TestsKernel.Models;

namespace Binacle.Lib.Benchmarks.Abstractions;

public abstract class BinParallelizationThresholdBenchmarkBase
{
    private LoopBinProcessor loopBinProcessor = null!;
    private ParallelBinProcessor parallelBinProcessor = null!;

    [Params(1, 2, 3, 4, 5, 6, 7)] 
    public int BinCount { get; set; }

    [Params(3, 7, 13, 17, 23, 29, 37, 47, 59, 67, 79)]
    public int ItemCount { get; set; }
    
    [Params(Algorithm.FFD, Algorithm.BFD, Algorithm.WFD)]
    public Algorithm Algorithm { get; set; }

    [ParamsSource(typeof(ConcurrencyProvider), nameof(ConcurrencyProvider.GetProcessorCount))]
    public int ProcessorCount { get; set; }

    public List<TestBin> Bins { get; set; } = null!;
    public List<TestItem> Items { get; set; } = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        this.loopBinProcessor = new LoopBinProcessor(this.AlgorithmFactory);
        this.parallelBinProcessor = new ParallelBinProcessor(this.AlgorithmFactory, this.ProcessorCount);
        this.Bins = SpecializedScalingProblemsProvider.GetBins(this.BinCount);
        this.Items = SpecializedScalingProblemsProvider.GetItems(this.ItemCount);
    }


    protected abstract IAlgorithmFactory AlgorithmFactory { get; }
    protected abstract AlgorithmOperation AlgorithmOperation { get; }

    [Benchmark(Baseline = true)]
    [BenchmarkOrder(10)]
    public IDictionary<string, OperationResult> Loop()
    {
        return this.loopBinProcessor.Process(
            this.Algorithm,
            this.Bins,
            this.Items, 
            new TestOperationParameters()
            {
                Operation = this.AlgorithmOperation
            });
    }

    [Benchmark]
    [BenchmarkOrder(20)]
    public IDictionary<string, OperationResult> Parallel()
    {
        return this.parallelBinProcessor.Process(
            this.Algorithm,
            this.Bins,
            this.Items, 
            new TestOperationParameters()
            {
                Operation = this.AlgorithmOperation
            });
    }
}
