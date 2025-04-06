using BenchmarkDotNet.Attributes;
using Binacle.Lib.Fitting.Models;
using Binacle.Lib;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.Benchmarks.Combination;

[MemoryDiagnoser]
public class FittingSingleBaseLine : MultipleBinsBenchmarkBase
{
	private readonly TestBin testBin;
	private readonly AlgorithmFactory algorithmFactory;

	public FittingSingleBaseLine()
	{
		// .Add("Rectangular-Cuboids::Medium", "BinaryDecision::Fits")
		this.algorithmFactory = new AlgorithmFactory();
		this.testBin = this.DataProvider
			.GetSuccessfulBins(this.BinCollectionsDataProvider)
			.FirstOrDefault(x => x.ID == "Medium")!;
		this.Items = this.DataProvider.GetItems();
	}
	
	/// 4,12, 24, 44, 58 
	[Params(1, 3, 5, 7, 9)]
	public int NoOfVariedItems { get; set; }
	public List<TestItem> Items { get; set; }

	[GlobalSetup]
	public void GlobalSetup()
	{
		this.Items = this.DataProvider
			.GetItems()
			.Take(this.NoOfVariedItems)
			.ToList();
	}

	[GlobalCleanup]
	public void GlobalCleanup()
	{
		this.Items = new List<TestItem>();
	}
	

	[Benchmark(Baseline = true)]
	public FittingResult FFD()
	{
		var algorithmInstance = this.algorithmFactory.CreateFitting(Algorithm.FirstFitDecreasing, this.testBin, this.Items);
		var result = algorithmInstance.Execute(new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		return result;
		
	}
	[Benchmark]
	public FittingResult WFD()
	{
		var algorithmInstance = this.algorithmFactory.CreateFitting(Algorithm.WorstFitDecreasing, this.testBin, this.Items);
		var result = algorithmInstance.Execute(new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		return result;
		
	}
	[Benchmark]
	public FittingResult BFD()
	{
		var algorithmInstance = this.algorithmFactory.CreateFitting(Algorithm.BestFitDecreasing, this.testBin, this.Items);
		var result = algorithmInstance.Execute(new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		return result;
	}
}
