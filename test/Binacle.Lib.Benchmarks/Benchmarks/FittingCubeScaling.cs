using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Fitting;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Lib.Fitting.Models;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.Benchmarks;

[MemoryDiagnoser]
public class FittingCubeScaling : CubeScalingBenchmarkBase
{
	public FittingCubeScaling() : base("Rectangular-Cuboids::Small")
	{
	}

	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public FittingResult FFD_v1()
		=> this.Run(AlgorithmFactories.Fitting_FFD_v1, this.Bin!, this.Items!);

	[Benchmark]
	[BenchmarkOrder(11)]
	public FittingResult FFD_v2()
		=> this.Run(AlgorithmFactories.Fitting_FFD_v2, this.Bin!, this.Items!);

	[Benchmark]
	[BenchmarkOrder(12)]
	public FittingResult FFD_v3()
		=> this.Run(AlgorithmFactories.Fitting_FFD_v3, this.Bin!, this.Items!);

	[Benchmark]
	[BenchmarkOrder(20)]
	public FittingResult WFD_v1()
		=> this.Run(AlgorithmFactories.Fitting_WFD_v1, this.Bin!, this.Items!);

	[Benchmark]
	[BenchmarkOrder(21)]
	public FittingResult WFD_v2()
		=> this.Run(AlgorithmFactories.Fitting_WFD_v2, this.Bin!, this.Items!);

	[Benchmark]
	[BenchmarkOrder(22)]
	public FittingResult WFD_v3()
		=> this.Run(AlgorithmFactories.Fitting_WFD_v3, this.Bin!, this.Items!);

	[Benchmark]
	[BenchmarkOrder(30)]
	public FittingResult BFD_v1()
		=> this.Run(AlgorithmFactories.Fitting_BFD_v1, this.Bin!, this.Items!);

	[Benchmark]
	[BenchmarkOrder(31)]
	public FittingResult BFD_v2()
		=> this.Run(AlgorithmFactories.Fitting_BFD_v2, this.Bin!, this.Items!);

	[Benchmark]
	[BenchmarkOrder(32)]
	public FittingResult BFD_v3()
		=> this.Run(AlgorithmFactories.Fitting_BFD_v3, this.Bin!, this.Items!);


	private FittingResult Run(AlgorithmFactory<IFittingAlgorithm> algorithmFactory, TestBin bin, List<TestItem> items)
	{
		var algorithmInstance = algorithmFactory(bin, items);
		var result = algorithmInstance.Execute(new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		return result;
	}
}
