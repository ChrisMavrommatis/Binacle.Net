using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions;
using Binacle.Lib.Benchmarks.Abstractions;

namespace Binacle.Lib.Benchmarks.AlgorithmRacing;

[MemoryDiagnoser]
public class AlgorithmRacing_Packing_v2 : AlgorithmRacingBenchmarksBase
{
    protected override AlgorithmOperation AlgorithmOperation =>
        AlgorithmOperation.Packing;

    protected override IAlgorithmFactory AlgorithmFactory => 
        new AlgorithmFactory_v2();
}