using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;

namespace Binacle.Lib.Benchmarks;

[MemoryDiagnoser]
public class AlgorithmVersion_Fitting_WFD : SingleBinCubeScalingBenchmarkBase
{
	public AlgorithmVersion_Fitting_WFD() : base("Rectangular-Cuboids::Small")
	{
	}

	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public OperationResult WFD_v1()
		=> this.Run(AlgorithmFactories.WFD_v1, AlgorithmOperation.Fitting, this.Bin!, this.Items!);

	[Benchmark]
	[BenchmarkOrder(20)]
	public OperationResult WFD_v2()
		=> this.Run(AlgorithmFactories.WFD_v2, AlgorithmOperation.Fitting, this.Bin!, this.Items!);
}
