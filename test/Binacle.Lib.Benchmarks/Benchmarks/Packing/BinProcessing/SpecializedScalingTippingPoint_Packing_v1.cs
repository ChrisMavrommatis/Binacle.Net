using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions;
using Binacle.Lib.Benchmarks.Abstractions;

namespace Binacle.Lib.Benchmarks.BinProcessing;

[MemoryDiagnoser]
public class SpecializedScalingTippingPoint_Packing_v1 : SpecializedScalingTippingPointBenchmarkBase
{
	protected override IAlgorithmFactory AlgorithmFactory =>
		new AlgorithmFactory_v1();
	
	protected override AlgorithmOperation AlgorithmOperation =>
		AlgorithmOperation.Packing;
}
