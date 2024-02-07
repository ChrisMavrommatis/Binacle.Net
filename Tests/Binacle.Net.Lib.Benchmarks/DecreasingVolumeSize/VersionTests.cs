using BenchmarkDotNet.Attributes;
using Binacle.Net.Lib.Benchmarks.Models;

namespace Binacle.Net.Lib.Benchmarks.DecreasingVolumeSize;

[MemoryDiagnoser]
public class VersionTests
{
    private readonly Dimensions<int> _5x5x5;
    private readonly List<TestBin> bins;

    private List<TestItem> items;

    public VersionTests()
    {
        this._5x5x5 = new Dimensions<int>(5, 5, 5);

        this.bins = new List<TestBin>()
        {
            new TestBin("Small", new Dimensions<int>(8,45,62)),
            new TestBin("Medium", new Dimensions<int>(17,45,62)),
            new TestBin("Large", new Dimensions<int>(36,45,62))
        };
    }

    [Params(10, 50, 100, 200, 500, 1000, 2000)]
    public int NumberOfItems;

    [GlobalSetup(Targets = new[] { nameof(Full_v1), nameof(Full_v2) })]
    public void FullGlobalSetup()
    {
        this.items = Enumerable.Range(1, NumberOfItems).Select(x => new TestItem(x.ToString(), _5x5x5)).ToList();
    }

    [Benchmark(Baseline = true)]
    public Lib.Models.BinFittingOperationResult Full_v1()
    {
        var strategy = new Strategies.DecreasingVolumeSize_v1()
            .WithBins(this.bins)
            .AndItems(this.items)
            .Build();

        return strategy.Execute();
    }

    [Benchmark]
    public Lib.Models.BinFittingOperationResult Full_v2()
    {
        var strategy = new Strategies.DecreasingVolumeSize_v2()
            .WithBins(this.bins)
            .AndItems(this.items)
            .Build();

        return strategy.Execute();
    }

}
