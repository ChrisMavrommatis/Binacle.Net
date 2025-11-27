using BenchmarkDotNet.Attributes;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Lib.Fitting.Models;

namespace Binacle.Lib.Benchmarks.AlgorithmVersion;

[MemoryDiagnoser]
public class Fitting_BFD : SingleBinCubeScalingBenchmarkBase
{
	public Fitting_BFD() : base("Rectangular-Cuboids::Small")
	{
	}


	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public FittingResult BFD_v1()
		=> this.Run(AlgorithmFactories.Fitting_BFD_v1, this.Bin!, this.Items!);

	[Benchmark]
	[BenchmarkOrder(20)]
	public FittingResult BFD_v2()
		=> this.Run(AlgorithmFactories.Fitting_BFD_v2, this.Bin!, this.Items!);

	[Benchmark]
	[BenchmarkOrder(30)]
	public FittingResult BFD_v3()
		=> this.Run(AlgorithmFactories.Fitting_BFD_v3, this.Bin!, this.Items!);
}
