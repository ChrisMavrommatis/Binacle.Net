using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Lib.ResultSelection;
using Binacle.TestsKernel.ResultSelection.Providers;

namespace Binacle.Lib.Benchmarks.Benchmarks.ResultSelection;

[MemoryDiagnoser]
public class BestAlgorithm_ResultSelection : ResultSelectionBenchmarkBase
{
    [ParamsSource(typeof(BestAlgorithmScenarioProvider), nameof(BestAlgorithmScenarioProvider.GetScenarioNames))]
    public override string? ScenarioName { get; set; }
    
    [Benchmark(Baseline = true)]
    [BenchmarkOrder(10)]
    public OperationResult v1()
        => this.Run(new BestAlgorithm_v1());

    [Benchmark]
    [BenchmarkOrder(20)]
    public OperationResult v2()
        => this.Run(new BestAlgorithm_v2());
}