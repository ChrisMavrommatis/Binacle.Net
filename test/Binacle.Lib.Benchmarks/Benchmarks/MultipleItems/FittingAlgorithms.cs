using BenchmarkDotNet.Attributes;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Lib.Fitting.Models;

namespace Binacle.Lib.Benchmarks.MultipleItems;

[MemoryDiagnoser]
public class FittingAlgorithms : MultipleItemsBenchmarkBase
{
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public IDictionary<string, FittingResult> FFD_Loop()
	{
		var results = this.LoopProcessor.ProcessFitting(Algorithm.FirstFitDecreasing, this.Bins, this.Items, new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		
		return results;
	}
	
	[Benchmark]
	[BenchmarkOrder(11)]
	public IDictionary<string, FittingResult> FFD_Parallel()
	{
		var results = this.ParallelProcessor.ProcessFitting(Algorithm.FirstFitDecreasing, this.Bins, this.Items, new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		
		return results;
	}
	
	[Benchmark]
	[BenchmarkOrder(20)]
	public IDictionary<string, FittingResult> WFD_Loop()
	{
		var results = this.LoopProcessor.ProcessFitting(Algorithm.WorstFitDecreasing, this.Bins, this.Items, new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		
		return results;
	}
	
	[Benchmark]
	[BenchmarkOrder(21)]
	public IDictionary<string, FittingResult> WFD_Parallel()
	{
		var results = this.ParallelProcessor.ProcessFitting(Algorithm.WorstFitDecreasing, this.Bins, this.Items, new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		
		return results;
	}

	
	[Benchmark]
	[BenchmarkOrder(30)]
	public IDictionary<string, FittingResult> BFD_Loop()
	{
		var results = this.LoopProcessor.ProcessFitting(Algorithm.BestFitDecreasing, this.Bins, this.Items, new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		
		return results;
	}
	
	[Benchmark]
	[BenchmarkOrder(31)]
	public IDictionary<string, FittingResult> BFD_Parallel()
	{
		var results = this.ParallelProcessor.ProcessFitting(Algorithm.BestFitDecreasing, this.Bins, this.Items, new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		
		return results;
	}
}
