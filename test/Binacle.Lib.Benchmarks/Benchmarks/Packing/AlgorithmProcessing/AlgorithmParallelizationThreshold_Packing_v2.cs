using Binacle.Lib.Abstractions;
using Binacle.Lib.Benchmarks.Abstractions;

namespace Binacle.Lib.Benchmarks.AlgorithmProcessing;

public class AlgorithmParallelizationThreshold_Packing_v2 : AlgorithmParallelizationThresholdBenchmarkBase
{
    protected override IAlgorithmFactory AlgorithmFactory =>
        new AlgorithmFactory_v2();
	
    protected override AlgorithmOperation AlgorithmOperation =>
        AlgorithmOperation.Packing;
}