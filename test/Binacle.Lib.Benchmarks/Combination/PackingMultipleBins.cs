using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;
using Binacle.Lib.Packing.Models;
using Binacle.Lib;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.Benchmarks.Combination;

[MemoryDiagnoser]
public class PackingMultipleBins : MultipleBinsBenchmarkBase
{
	private readonly ParallelBinProcessor parallelProcessor;
	private readonly LoopBinProcessor loopProcessor;

	public PackingMultipleBins()
	{
		var algorithmFactory = new AlgorithmFactory();
		this.loopProcessor = new LoopBinProcessor(algorithmFactory);
		this.parallelProcessor = new ParallelBinProcessor(algorithmFactory);
		this.Bins = new List<TestBin>();
		this.Items = this.DataProvider.GetItems();
		// this.Items = new List<TestItem>();
	}

	[Params(2, 8, 14, 20, 26, 32, 38)]
	public int NoOfBins { get; set; }
	public List<TestBin> Bins { get; set; }
	
	// /// 4,12, 24, 44, 58 
	// [Params(1, 3, 5, 7, 9)]
	// public int NoOfVariedItems { get; set; }
	public List<TestItem> Items { get; set; }

	[GlobalSetup]
	public void GlobalSetup()
	{
		this.Bins = this.DataProvider
			.GetSuccessfulBins(this.BinCollectionsDataProvider)
			.Take(this.NoOfBins)
			.ToList();
		// this.Items = this.DataProvider
		// 	.GetItems()
		// 	.Take(this.NoOfVariedItems)
		// 	.ToList();
	}

	[GlobalCleanup]
	public void GlobalCleanup()
	{
		this.Bins = new List<TestBin>();
		// this.Items = new List<TestItem>();
	}
	
	[Benchmark(Baseline = true)]
	public IDictionary<string, PackingResult> FFD_Loop()
	{
		var results = this.loopProcessor.ProcessPacking(Algorithm.FirstFitDecreasing, this.Bins, this.Items, new PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		
		return results;
	}
	
	[Benchmark]
	public IDictionary<string, PackingResult> FFD_Parallel()
	{
		var results = this.parallelProcessor.ProcessPacking(Algorithm.FirstFitDecreasing, this.Bins, this.Items, new PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		
		return results;
	}
	
	[Benchmark]
	public IDictionary<string, PackingResult> WFD_Loop()
	{
		var results = this.loopProcessor.ProcessPacking(Algorithm.WorstFitDecreasing, this.Bins, this.Items, new PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		
		return results;
	}
	
	[Benchmark]
	public IDictionary<string, PackingResult> WFD_Parallel()
	{
		var results = this.parallelProcessor.ProcessPacking(Algorithm.WorstFitDecreasing, this.Bins, this.Items, new PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		
		return results;
	}

	
	[Benchmark]
	public IDictionary<string, PackingResult> BFD_Loop()
	{
		var results = this.loopProcessor.ProcessPacking(Algorithm.BestFitDecreasing, this.Bins, this.Items, new PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		
		return results;
	}
	
	[Benchmark]
	public IDictionary<string, PackingResult> BFD_Parallel()
	{
		var results = this.parallelProcessor.ProcessPacking(Algorithm.BestFitDecreasing, this.Bins, this.Items, new PackingParameters
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		
		return results;
	}
}
