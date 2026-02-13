using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.TestsKernel;

namespace Binacle.Lib.Benchmarks.AlgorithmRacing;

[MemoryDiagnoser]
public class AlgorithmRacing_Packing_FFD_BFD_v1 : AlgorithmRacingBase
{
	protected override TestAlgorithmFactory<IPackingAlgorithm>[] Algorithms =>
	[
		AlgorithmFactories.FFD_v1,
		AlgorithmFactories.BFD_v1,
	];

	protected override AlgorithmOperation AlgorithmOperation =>
		AlgorithmOperation.Packing;
}
