using BenchmarkDotNet.Attributes;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Lib.Packing.Models;

namespace Binacle.Lib.Benchmarks.AlgorithmVersion;

[MemoryDiagnoser]
public class Packing_FFD : SingleBinCubeScalingBenchmarkBase
{
	public Packing_FFD() : base("Rectangular-Cuboids::Small")
	{
	}

	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public PackingResult FFD_v1()
		=> this.Run(AlgorithmFactories.Packing_FFD_v1, this.Bin!, this.Items!);
	
	[Benchmark]
	[BenchmarkOrder(20)]
	public PackingResult FFD_v2()
		=> this.Run(AlgorithmFactories.Packing_FFD_v2, this.Bin!, this.Items!);
}
