using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.TestsKernel;

namespace Binacle.Lib.Benchmarks.AlgorithmRacing;

[MemoryDiagnoser]
public class AlgorithmRacing_Packing_FFD_WFD_v2 : AlgorithmRacingBase
{
	protected override TestAlgorithmFactory<IPackingAlgorithm>[] Algorithms =>
	[
		AlgorithmFactories.FFD_v2,
		AlgorithmFactories.WFD_v2
	];

	protected override AlgorithmOperation AlgorithmOperation =>
		AlgorithmOperation.Packing;
}
