using BenchmarkDotNet.Attributes;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Lib.Packing.Models;

namespace Binacle.Lib.Benchmarks;

[MemoryDiagnoser]
public class AlgorithmVersion_Packing_WFD : SingleBinCubeScalingBenchmarkBase
{
	public AlgorithmVersion_Packing_WFD() : base("Rectangular-Cuboids::Small")
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
