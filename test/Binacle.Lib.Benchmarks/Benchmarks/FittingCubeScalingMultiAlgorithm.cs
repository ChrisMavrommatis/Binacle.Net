// using BenchmarkDotNet.Attributes;
// using Binacle.Lib.Abstractions.Algorithms;
// using Binacle.Lib.Abstractions.Fitting;
// using Binacle.Lib.Benchmarks.Abstractions;
// using Binacle.Lib.Benchmarks.Order;
// using Binacle.Lib.Fitting.Models;
// using Binacle.Lib.Packing.Models;
// using Binacle.Net.TestsKernel.Models;
//
// namespace Binacle.Lib.Benchmarks;
//
// [MemoryDiagnoser]
// public class FittingCubeScalingMultiAlgorithm : CubeScalingBenchmarkBase
// {
// 	public FittingCubeScalingMultiAlgorithm() : base("Rectangular-Cuboids::Small")
// 	{
// 	}
//
// 	private readonly AlgorithmFactory<IFittingAlgorithm>[] algorithmFactories_v1 =
// 	[
// 		AlgorithmFactories.Fitting_FFD_v1,
// 		AlgorithmFactories.Fitting_WFD_v1,
// 		AlgorithmFactories.Fitting_BFD_v1,
// 	];
// 	
// 	private readonly AlgorithmFactory<IFittingAlgorithm>[] algorithmFactories_v2 =
// 	[
// 		AlgorithmFactories.Fitting_FFD_v2,
// 		AlgorithmFactories.Fitting_WFD_v2,
// 		AlgorithmFactories.Fitting_BFD_v2,
// 	];
// 	
// 	private readonly AlgorithmFactory<IFittingAlgorithm>[] algorithmFactories_v3 =
// 	[
// 		AlgorithmFactories.Fitting_FFD_v2,
// 		AlgorithmFactories.Fitting_WFD_v2,
// 		AlgorithmFactories.Fitting_BFD_v2,
// 	];
// 	
// 	[Benchmark(Baseline = true)]
// 	[BenchmarkOrder(10)]
// 	public FittingResult ForLoop_v1()
// 	{
// 		var results = new FittingResult[this.algorithmFactories_v1.Length];
// 		for (var i = 0; i < this.algorithmFactories_v1.Length; i++)
// 		{
// 			var algorithmFactory = this.algorithmFactories_v1[i];
// 			var algorithmInstance = algorithmFactory(this.Bin!, this.Items!);
// 			var result = algorithmInstance.Execute(new FittingParameters()
// 			{
// 				ReportFittedItems = false,
// 				ReportUnfittedItems =false,
// 			});
// 			results[i] = result;
// 		}
//
// 		return results
// 			.OrderByDescending(x => x.Status == FittingResultStatus.Success ? x.FittedItemsVolumePercentage : 0)
// 			.FirstOrDefault()!;
// 	}
//
// 	[Benchmark]
// 	[BenchmarkOrder(11)]
// 	public FittingResult ParallelFor_v1()
// 	{
// 		var results = new FittingResult[this.algorithmFactories_v1.Length];
// 		Parallel.For(0, this.algorithmFactories_v1.Length, index =>
// 		{
// 			var algorithmFactory = this.algorithmFactories_v1[index];
// 			var algorithmInstance = algorithmFactory(this.Bin!, this.Items!);
// 			var result = algorithmInstance.Execute(new FittingParameters
// 			{
// 				ReportFittedItems = false,
// 				ReportUnfittedItems =false,
// 			});
// 			results[index] = result;
// 		});
// 		return results
// 			.OrderByDescending(x => x.Status == FittingResultStatus.Success ? x.FittedItemsVolumePercentage : 0)
// 			.FirstOrDefault()!;
// 	}
// 	
// 	[Benchmark]
// 	[BenchmarkOrder(20)]
// 	public FittingResult ForLoop_v2()
// 	{
// 		var results = new FittingResult[this.algorithmFactories_v2.Length];
// 		for (var i = 0; i < this.algorithmFactories_v2.Length; i++)
// 		{
// 			var algorithmFactory = this.algorithmFactories_v2[i];
// 			var algorithmInstance = algorithmFactory(this.Bin!, this.Items!);
// 			var result = algorithmInstance.Execute(new FittingParameters()
// 			{
// 				ReportFittedItems = false,
// 				ReportUnfittedItems =false,
// 			});
// 			results[i] = result;
// 		}
//
// 		return results
// 			.OrderByDescending(x => x.Status == FittingResultStatus.Success ? x.FittedItemsVolumePercentage : 0)
// 			.FirstOrDefault()!;
// 	}
//
// 	[Benchmark]
// 	[BenchmarkOrder(21)]
// 	public FittingResult ParallelFor_v2()
// 	{
// 		var results = new FittingResult[this.algorithmFactories_v2.Length];
// 		Parallel.For(0, this.algorithmFactories_v2.Length, index =>
// 		{
// 			var algorithmFactory = this.algorithmFactories_v2[index];
// 			var algorithmInstance = algorithmFactory(this.Bin!, this.Items!);
// 			var result = algorithmInstance.Execute(new FittingParameters
// 			{
// 				ReportFittedItems = false,
// 				ReportUnfittedItems =false,
// 			});
// 			results[index] = result;
// 		});
// 		return results
// 			.OrderByDescending(x => x.Status == FittingResultStatus.Success ? x.FittedItemsVolumePercentage : 0)
// 			.FirstOrDefault()!;
// 	}
// 	
// 	[Benchmark]
// 	[BenchmarkOrder(30)]
// 	public FittingResult ForLoop_v3()
// 	{
// 		var results = new FittingResult[this.algorithmFactories_v3.Length];
// 		for (var i = 0; i < this.algorithmFactories_v3.Length; i++)
// 		{
// 			var algorithmFactory = this.algorithmFactories_v3[i];
// 			var algorithmInstance = algorithmFactory(this.Bin!, this.Items!);
// 			var result = algorithmInstance.Execute(new FittingParameters()
// 			{
// 				ReportFittedItems = false,
// 				ReportUnfittedItems =false,
// 			});
// 			results[i] = result;
// 		}
//
// 		return results
// 			.OrderByDescending(x => x.Status == FittingResultStatus.Success ? x.FittedItemsVolumePercentage : 0)
// 			.FirstOrDefault()!;
// 	}
//
// 	[Benchmark]
// 	[BenchmarkOrder(31)]
// 	public FittingResult ParallelFor_v3()
// 	{
// 		var results = new FittingResult[this.algorithmFactories_v3.Length];
// 		Parallel.For(0, this.algorithmFactories_v3.Length, index =>
// 		{
// 			var algorithmFactory = this.algorithmFactories_v3[index];
// 			var algorithmInstance = algorithmFactory(this.Bin!, this.Items!);
// 			var result = algorithmInstance.Execute(new FittingParameters
// 			{
// 				ReportFittedItems = false,
// 				ReportUnfittedItems =false,
// 			});
// 			results[index] = result;
// 		});
// 		return results
// 			.OrderByDescending(x => x.Status == FittingResultStatus.Success ? x.FittedItemsVolumePercentage : 0)
// 			.FirstOrDefault()!;
// 	}
// }
