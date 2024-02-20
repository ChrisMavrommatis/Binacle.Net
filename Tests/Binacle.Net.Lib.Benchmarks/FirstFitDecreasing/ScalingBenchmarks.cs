using BenchmarkDotNet.Attributes;
using Binacle.Net.Lib.Benchmarks.Data.Providers;

namespace Binacle.Net.Lib.Benchmarks.FirstFitDecreasing;

[MemoryDiagnoser]
public class ScalingBenchmarks
{
    private BinsDataProvider binsDataProvider;

    public ScalingBenchmarks()
    {
        
    }

    [GlobalSetup]
    public void GlobalSetup()
    {
        this.binsDataProvider = new Data.Providers.BinsDataProvider();
    }


    //[GlobalSetup(Targets = new[] { nameof(Full_v1), nameof(Full_v2) })]
    //public void FullGlobalSetup()
    //{
    //    this.items = Enumerable.Range(1, NumberOfItems).Select(x => new TestItem(x.ToString(), _5x5x5)).ToList();
    //}

    //[Benchmark(Baseline = true)]
    //public Lib.Models.BinFittingOperationResult Full_v1()
    //{
    //    var strategy = new Strategies.FirstFitDecreasing_v1()
    //        .WithBins()
    //        .AndItems(this.items)
    //        .Build();

    //    return strategy.Execute();
    //}

    //[Benchmark]
    //public Lib.Models.BinFittingOperationResult Full_v2()
    //{
    //    var strategy = new Strategies.FirstFitDecreasing_v2()
    //        .WithBins(this.bins)
    //        .AndItems(this.items)
    //        .Build();

    //    return strategy.Execute();
    //}

}
