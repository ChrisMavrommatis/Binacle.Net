using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;

namespace Binacle.Lib.Benchmarks;

[MemoryDiagnoser]
public class AlgorithmVersion_Packing_WFD : SingleBinCubeScalingBenchmarkBase
{
	public AlgorithmVersion_Packing_WFD() : base("Rectangular-Cuboids::Small")
	{
	}

	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public OperationResult WFD_v1()
		=> this.Run(AlgorithmFactories.WFD_v1, AlgorithmOperation.Packing, this.Bin!, this.Items!);

	[Benchmark]
	[BenchmarkOrder(20)]
	public OperationResult WFD_v2()
		=> this.Run(AlgorithmFactories.WFD_v2, AlgorithmOperation.Packing, this.Bin!, this.Items!);
}
