using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;

namespace Binacle.Lib.Benchmarks;

[MemoryDiagnoser]
public class AlgorithmVersion_Fitting_BFD : SingleBinCubeScalingBenchmarkBase
{
	public AlgorithmVersion_Fitting_BFD() : base("Rectangular-Cuboids::Small")
	{
	}


	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public OperationResult BFD_v1()
		=> this.Run(AlgorithmFactories.BFD_v1, AlgorithmOperation.Fitting, this.Bin!, this.Items!);

	[Benchmark]
	[BenchmarkOrder(20)]
	public OperationResult BFD_v2()
		=> this.Run(AlgorithmFactories.BFD_v2, AlgorithmOperation.Fitting, this.Bin!, this.Items!);
}
