using BenchmarkDotNet.Attributes;
using Binacle.Lib.Benchmarks.Abstractions;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Lib.Packing.Models;

namespace Binacle.Lib.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class MultipleBins_PackingAlgorithms : MultipleBinsBenchmarkBase
{
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public IDictionary<string, PackingResult> FFD_Loop()
	{
		var results = this.LoopProcessor.ProcessPacking(Algorithm.FirstFitDecreasing, this.Bins, this.Items, new PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		
		return results;
	}
	
	[Benchmark]
	[BenchmarkOrder(11)]
	public IDictionary<string, PackingResult> FFD_Parallel()
	{
		var results = this.ParallelProcessor.ProcessPacking(Algorithm.FirstFitDecreasing, this.Bins, this.Items, new PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		
		return results;
	}
	
	[Benchmark]
	[BenchmarkOrder(20)]
	public IDictionary<string, PackingResult> WFD_Loop()
	{
		var results = this.LoopProcessor.ProcessPacking(Algorithm.WorstFitDecreasing, this.Bins, this.Items, new PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		
		return results;
	}
	
	[Benchmark]
	[BenchmarkOrder(21)]
	public IDictionary<string, PackingResult> WFD_Parallel()
	{
		var results = this.ParallelProcessor.ProcessPacking(Algorithm.WorstFitDecreasing, this.Bins, this.Items, new PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		
		return results;
	}

	
	[Benchmark]
	[BenchmarkOrder(30)]
	public IDictionary<string, PackingResult> BFD_Loop()
	{
		var results = this.LoopProcessor.ProcessPacking(Algorithm.BestFitDecreasing, this.Bins, this.Items, new PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		
		return results;
	}
	
	[Benchmark]
	[BenchmarkOrder(31)]
	public IDictionary<string, PackingResult> BFD_Parallel()
	{
		var results = this.ParallelProcessor.ProcessPacking(Algorithm.BestFitDecreasing, this.Bins, this.Items, new PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		
		return results;
	}
}
