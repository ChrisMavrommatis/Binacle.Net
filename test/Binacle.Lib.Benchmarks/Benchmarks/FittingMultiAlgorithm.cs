// using BenchmarkDotNet.Attributes;
// using Binacle.Lib.Benchmarks.Abstractions;
// using Binacle.Lib.Benchmarks.Order;
// using Binacle.Lib.Fitting.Models;
//
// namespace Binacle.Lib.Benchmarks;
//
// [MemoryDiagnoser]
// public class FittingMultiAlgorithm : MultipleBinsBenchmarkBase
// {
// 	[Benchmark(Baseline = true)]
// 	[BenchmarkOrder(10)]
// 	public IDictionary<string, FittingResult[]> Loop()
// 	{
// 		var results = this.LoopProcessor.ProcessFitting(this.Bins, this.Items, new FittingParameters
// 		{
// 			ReportFittedItems = false,
// 			ReportUnfittedItems = false
// 		});
// 		
// 		return results;
// 	}
// 	
// 	[Benchmark]
// 	[BenchmarkOrder(11)]
// 	public IDictionary<string, FittingResult[]> Parallel()
// 	{
// 		var results = this.ParallelProcessor.ProcessFitting(this.Bins, this.Items, new FittingParameters
// 		{
// 			ReportFittedItems = false,
// 			ReportUnfittedItems = false
// 		});
// 		
// 		return results;
// 	}
// }
