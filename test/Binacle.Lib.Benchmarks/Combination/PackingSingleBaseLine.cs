using BenchmarkDotNet.Attributes;
using Binacle.Lib.Benchmarks.Order;
using Binacle.Lib.Packing.Models;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.Benchmarks.Combination;

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
	[BenchmarkOrder(10)]
	public PackingResult FFD_v1()
	{
		var algorithmInstance = AlgorithmFactories.Packing_FFD_v1(this.testBin!, this.Items!);
		var result = algorithmInstance.Execute(new PackingParameters()
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems =false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		return result;
	}
	
	[Benchmark]
	[BenchmarkOrder(11)]
	public PackingResult FFD_v2()
	{
		var algorithmInstance = AlgorithmFactories.Packing_FFD_v2(this.testBin!, this.Items!);
		var result = algorithmInstance.Execute(new PackingParameters()
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems =false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		return result;
	}
	
	[Benchmark]
	[BenchmarkOrder(20)]
	public PackingResult WFD_v1()
	{
		var algorithmInstance = AlgorithmFactories.Packing_WFD_v1(this.testBin!, this.Items!);
		var result = algorithmInstance.Execute(new PackingParameters()
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems =false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		return result;
	}
	
	[Benchmark]
	[BenchmarkOrder(21)]
	public PackingResult WFD_v2()
	{
		var algorithmInstance = AlgorithmFactories.Packing_WFD_v2(this.testBin!, this.Items!);
		var result = algorithmInstance.Execute(new PackingParameters()
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems =false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		return result;
	}
	
	[Benchmark]
	[BenchmarkOrder(30)]
	public PackingResult BFD_v1()
	{
		var algorithmInstance = AlgorithmFactories.Packing_BFD_v1(this.testBin!, this.Items!);
		var result = algorithmInstance.Execute(new PackingParameters()
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems =false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		return result;
	}
	
	[Benchmark]
	[BenchmarkOrder(31)]
	public PackingResult BFD_v2()
	{
		var algorithmInstance = AlgorithmFactories.Packing_BFD_v2(this.testBin!, this.Items!);
		var result = algorithmInstance.Execute(new PackingParameters()
		{
			OptInToEarlyFails = false,
			NeverReportUnpackedItems =false,
			ReportPackedItemsOnlyWhenFullyPacked = false
		});
		return result;
	}
}
