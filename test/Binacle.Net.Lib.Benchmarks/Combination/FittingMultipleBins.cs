using BenchmarkDotNet.Attributes;
using Binacle.Net.Lib.Fitting.Models;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Net.Lib.Benchmarks.Combination;

[MemoryDiagnoser]
public class FittingMultipleBins : MultipleBinsBenchmarkBase
{
	private readonly ParallelBinProcessor parallelProcessor;
	private readonly LoopBinProcessor loopProcessor;

	public FittingMultipleBins()
	{
		this.loopProcessor = new LoopBinProcessor();
		this.parallelProcessor = new ParallelBinProcessor();
		this.Bins = new List<TestBin>();
		this.Items = new List<TestItem>();
	}

	[Params(2, 8, 14, 20, 26, 32, 38)]
	public int NoOfBins { get; set; }
	public List<TestBin> Bins { get; set; }
	
	/// 4,12, 24, 44, 58 
	[Params(1, 3, 5, 7, 9)]
	public int NoOfVariedItems { get; set; }
	public List<TestItem> Items { get; set; }
	
	[GlobalSetup]
	public void GlobalSetup()
	{
		this.Bins = this.DataProvider
			.GetSuccessfulBins(this.BinCollectionsDataProvider)
			.Take(this.NoOfBins)
			.ToList();
		
		this.Items = this.DataProvider
			.GetItems()
			.Take(this.NoOfVariedItems)
			.ToList();
	}

	[GlobalCleanup]
	public void GlobalCleanup()
	{
		this.Bins = new List<TestBin>();
		this.Items = new List<TestItem>();
	}
	
	[Benchmark(Baseline = true)]
	public Dictionary<string, FittingResult> FFD_Loop()
	{
		var results = this.loopProcessor.ProcessFitting(Algorithm.FirstFitDecreasing, this.Bins, this.Items, new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		
		return results;
	}
	
	[Benchmark]
	public Dictionary<string, FittingResult> FFD_Parallel()
	{
		var results = this.parallelProcessor.ProcessFitting(Algorithm.FirstFitDecreasing, this.Bins, this.Items, new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		
		return results;
	}
	
	[Benchmark]
	public Dictionary<string, FittingResult> WFD_Loop()
	{
		var results = this.loopProcessor.ProcessFitting(Algorithm.WorstFitDecreasing, this.Bins, this.Items, new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		
		return results;
	}
	
	[Benchmark]
	public Dictionary<string, FittingResult> WFD_Parallel()
	{
		var results = this.parallelProcessor.ProcessFitting(Algorithm.WorstFitDecreasing, this.Bins, this.Items, new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		
		return results;
	}

	
	[Benchmark]
	public Dictionary<string, FittingResult> BFD_Loop()
	{
		var results = this.loopProcessor.ProcessFitting(Algorithm.BestFitDecreasing, this.Bins, this.Items, new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		
		return results;
	}
	
	[Benchmark]
	public Dictionary<string, FittingResult> BFD_Parallel()
	{
		var results = this.parallelProcessor.ProcessFitting(Algorithm.BestFitDecreasing, this.Bins, this.Items, new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		
		return results;
	}
}
