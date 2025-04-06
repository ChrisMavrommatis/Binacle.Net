using BenchmarkDotNet.Attributes;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Lib.Fitting.Models;

namespace Binacle.Lib.Benchmarks.Scaling;

[MemoryDiagnoser]
public class FittingComparison : SingleScenarioScalingBase
{
	public FittingComparison() : base("Rectangular-Cuboids::Small")
	{
	}
	
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(0)]
	public FittingResult Fitting_FFD_v1()
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
	[BenchmarkOrder(1)]
	public FittingResult Fitting_WFD_v1()
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
	[BenchmarkOrder(2)]
	public FittingResult Fitting_BFD_v1()
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
