using BenchmarkDotNet.Attributes;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Lib.Packing.Models;

namespace Binacle.Lib.Benchmarks;

[MemoryDiagnoser]
public class AlgorithmVersion_Packing_BFD : SingleBinCubeScalingBenchmarkBase
{
	public AlgorithmVersion_Packing_BFD() : base("Rectangular-Cuboids::Small")
	{
	}

	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public PackingResult BFD_v1()
		=> this.Run(AlgorithmFactories.Packing_BFD_v1, this.Bin!, this.Items!);

	[Benchmark]
	[BenchmarkOrder(20)]
	public PackingResult BFD_v2()
		=> this.Run(AlgorithmFactories.Packing_BFD_v2, this.Bin!, this.Items!);
}
