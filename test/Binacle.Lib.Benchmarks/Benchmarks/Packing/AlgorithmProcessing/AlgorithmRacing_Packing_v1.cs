using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions;
using Binacle.Lib.Benchmarks.Abstractions;

namespace Binacle.Lib.Benchmarks.AlgorithmProcessing;

[MemoryDiagnoser]
public class AlgorithmRacing_Packing_v1 : AlgorithmRacingBenchmarksBase
{
	protected override AlgorithmOperation AlgorithmOperation =>
		AlgorithmOperation.Packing;

	protected override IAlgorithmFactory AlgorithmFactory => 
		new AlgorithmFactory_v1();
}
