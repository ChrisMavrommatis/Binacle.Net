using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Models;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;

namespace Binacle.Lib.Benchmarks;

[MemoryDiagnoser]
public class AlgorithmVersion_Fitting_FFD : SingleBinCubeScalingBenchmarkBase
{
	public AlgorithmVersion_Fitting_FFD() : base("Rectangular-Cuboids::Small")
	{
	}

	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public OperationResult FFD_v1()
		=> this.Run(AlgorithmFactories.FFD_v1, AlgorithmOperation.Fitting, this.Bin!, this.Items!);

	[Benchmark]
	[BenchmarkOrder(20)]
	public OperationResult FFD_v2()
		=> this.Run(AlgorithmFactories.FFD_v2, AlgorithmOperation.Fitting, this.Bin!, this.Items!);

	
}
