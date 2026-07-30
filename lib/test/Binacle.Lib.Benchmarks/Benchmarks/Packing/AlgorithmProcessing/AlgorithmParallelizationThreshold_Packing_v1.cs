using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions;
using Binacle.Lib.Benchmarks.Abstractions;

namespace Binacle.Lib.Benchmarks.AlgorithmProcessing;

[MemoryDiagnoser]
public class AlgorithmParallelizationThreshold_Packing_v1 : AlgorithmParallelizationThresholdBenchmarkBase
{
    protected override IAlgorithmFactory AlgorithmFactory =>
        new AlgorithmFactory_v1();
	
    protected override AlgorithmOperation AlgorithmOperation =>
        AlgorithmOperation.Packing;
}
