using BenchmarkDotNet.Attributes;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Lib.Packing.Models;

namespace Binacle.Lib.Benchmarks.AlgorithmVersion;

[MemoryDiagnoser]
public class Packing_WFD : SingleBinCubeScalingBenchmarkBase
{
	public Packing_WFD() : base("Rectangular-Cuboids::Small")
	{
	}

	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public PackingResult WFD_v1()
		=> this.Run(AlgorithmFactories.Packing_WFD_v1, this.Bin!, this.Items!);

	[Benchmark]
	[BenchmarkOrder(20)]
	public PackingResult WFD_v2()
		=> this.Run(AlgorithmFactories.Packing_WFD_v2, this.Bin!, this.Items!);
}
