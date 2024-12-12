using BenchmarkDotNet.Attributes;
using Binacle.Net.Lib.Packing.Models;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Net.Lib.Benchmarks.Combination;

[MemoryDiagnoser]
public class PackingSingleBaseLine : MultipleBinsBenchmarkBase
{
	private readonly TestBin testBin;
	private readonly AlgorithmFactory algorithmFactory;

	public PackingSingleBaseLine()
	{
		// .Add("Rectangular-Cuboids::Medium", "BinaryDecision::Fits")
		this.algorithmFactory = new AlgorithmFactory();
		this.testBin = this.DataProvider
			.GetSuccessfulBins(this.BinCollectionsDataProvider)
			.FirstOrDefault(x => x.ID == "Medium")!;
		this.Items = new List<TestItem>();
	}
	
	/// 4,12, 24, 44, 58 
	[Params(1, 3, 5, 7, 9)]
	public int NoOfVariedItems { get; set; }
	
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
	public List<TestItem> Items { get; set; }

	[Benchmark(Baseline = true)]
	public PackingResult FFD()
	{
		var algorithmInstance = this.algorithmFactory.CreatePacking(Algorithm.FirstFitDecreasing, this.testBin, this.Items);
		var result = algorithmInstance.Execute(new PackingParameters()
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems =false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		return result;
		
	}
	[Benchmark]
	public PackingResult WFD()
	{
		var algorithmInstance = this.algorithmFactory.CreatePacking(Algorithm.WorstFitDecreasing, this.testBin, this.Items);
		var result = algorithmInstance.Execute(new PackingParameters()
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems =false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		return result;
		
	}
	[Benchmark]
	public PackingResult BFD()
	{
		var algorithmInstance = this.algorithmFactory.CreatePacking(Algorithm.BestFitDecreasing, this.testBin, this.Items);
		var result = algorithmInstance.Execute(new PackingParameters()
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems =false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		return result;
	}
}
