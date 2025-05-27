using BenchmarkDotNet.Attributes;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Lib.Fitting.Models;

namespace Binacle.Lib.Benchmarks;

[MemoryDiagnoser]
public class FittingCubeScaling: CubeScalingBenchmarkBase
{
	public FittingCubeScaling() : base("Rectangular-Cuboids::Small")
	{
	}
	
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public FittingResult FFD_v1()
	{
		var algorithmInstance = AlgorithmFactories.Fitting_FFD_v1(this.Bin!, this.Items!);
		var result = algorithmInstance.Execute(new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		return result;
	}
	
	[Benchmark]
	[BenchmarkOrder(11)]
	public FittingResult FFD_v2()
	{
		var algorithmInstance = AlgorithmFactories.Fitting_FFD_v2(this.Bin!, this.Items!);
		var result = algorithmInstance.Execute(new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		return result;
	}
	
	[Benchmark]
	[BenchmarkOrder(13)]
	public FittingResult FFD_v3()
	{
		var algorithmInstance = AlgorithmFactories.Fitting_FFD_v3(this.Bin!, this.Items!);
		var result = algorithmInstance.Execute(new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		return result;
	}
	
	[Benchmark]
	[BenchmarkOrder(20)]
	public FittingResult WFD_v1()
	{
		var algorithmInstance = AlgorithmFactories.Fitting_WFD_v1(this.Bin!, this.Items!);
		var result = algorithmInstance.Execute(new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		return result;
	}

	[Benchmark]
	[BenchmarkOrder(30)]
	public FittingResult BFD_v1()
	{
		var algorithmInstance = AlgorithmFactories.Fitting_BFD_v1(this.Bin!, this.Items!);
		var result = algorithmInstance.Execute(new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		return result;
	}
}
