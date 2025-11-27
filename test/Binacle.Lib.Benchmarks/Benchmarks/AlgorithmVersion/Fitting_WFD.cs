using BenchmarkDotNet.Attributes;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Lib.Fitting.Models;

namespace Binacle.Lib.Benchmarks.AlgorithmVersion;

[MemoryDiagnoser]
public class Fitting_WFD : SingleBinCubeScalingBenchmarkBase
{
	public Fitting_WFD() : base("Rectangular-Cuboids::Small")
	{
	}

	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public FittingResult WFD_v1()
		=> this.Run(AlgorithmFactories.Fitting_WFD_v1, this.Bin!, this.Items!);

	[Benchmark]
	[BenchmarkOrder(20)]
	public FittingResult WFD_v2()
		=> this.Run(AlgorithmFactories.Fitting_WFD_v2, this.Bin!, this.Items!);

	[Benchmark]
	[BenchmarkOrder(30)]
	public FittingResult WFD_v3()
		=> this.Run(AlgorithmFactories.Fitting_WFD_v3, this.Bin!, this.Items!);
}
