using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.TestsKernel;

namespace Binacle.Lib.Benchmarks.BinProcessing;

[MemoryDiagnoser]
public class TippingPoint_Packing_BFD_v1 : TippingPointBenchmarkBase
{
	protected override TestAlgorithmFactory<IPackingAlgorithm> AlgorithmFactory
		=> AlgorithmFactories.BFD_v1;

	protected override AlgorithmOperation AlgorithmOperation
		=> AlgorithmOperation.Packing;
}
