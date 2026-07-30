using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions;
using Binacle.Lib.Benchmarks.Abstractions;

namespace Binacle.Lib.Benchmarks.BinProcessing;

[MemoryDiagnoser]
public class BinParallelizationThreshold_Packing_v2 : BinParallelizationThresholdBenchmarkBase
{
    protected override IAlgorithmFactory AlgorithmFactory =>
        new AlgorithmFactory_v2();
	
    protected override AlgorithmOperation AlgorithmOperation =>
        AlgorithmOperation.Packing;
}
