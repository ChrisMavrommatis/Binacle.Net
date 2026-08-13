using BenchmarkDotNet.Attributes;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Lib.ResultSelection;
using Binacle.Lib.TestsKernel.ResultSelection.Providers;

namespace Binacle.Lib.Benchmarks.Benchmarks.ResultSelection;

[MemoryDiagnoser]
public class BestBin_ResultSelection : ResultSelectionBenchmarkBase
{
    [ParamsSource(typeof(BestBinScenarioProvider), nameof(BestBinScenarioProvider.GetScenarioNames))]
    public override string? ScenarioName { get; set; }
    
    [Benchmark(Baseline = true)]
    [BenchmarkOrder(10)]
    public OperationResult v1()
        => this.Run(new BestBin_v1());

    [Benchmark]
    [BenchmarkOrder(20)]
    public OperationResult v2()
        => this.Run(new BestBin_v2());
}