using BenchmarkDotNet.Attributes;
using Binacle.Net.Lib.Fitting.Models;
using Binacle.Net.Lib.Packing.Models;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;
using System.Runtime.CompilerServices;

namespace Binacle.Net.Lib.Benchmarks.FirstFitDecreasing;

[MemoryDiagnoser]
public class ScalingBenchmarks
{
	static string GetSolutionRoot([CallerFilePath] string callerFilePath = "")
	{
		//go ../../ from callerFilePath
		var callerDirectory = Path.GetDirectoryName(callerFilePath);
		var solutionRoot = Path.GetFullPath(Path.Combine(callerDirectory, "..", "..", ".."));
		return solutionRoot;
	}

	[ParamsSource(nameof(NoOfItemsParamsSourceAccessor))]
	public int NoOfItems { get; set; }

	public static IEnumerable<int> NoOfItemsParamsSourceAccessor() => BenchmarkScalingTestsDataProvider.GetNoOfItems();

	[GlobalSetup]
	public void GlobalSetup()
	{
		var _5x5x5 = BenchmarkScalingTestsDataProvider.GetDimensions();

		this.rundataProvider = new BinCollectionsTestDataProvider(solutionRootBasePath: GetSolutionRoot());
		this.bins = this.rundataProvider.GetCollection(BenchmarkScalingTestsDataProvider.BinCollectionName);
		this.items = Enumerable.Range(1, this.NoOfItems).Select(x => new TestItem(x.ToString(), _5x5x5)).ToList();
		
	}

	[GlobalCleanup]
	public void GlobalCleanup()
	{
		this.rundataProvider = null;
		this.items = null;
		this.bins = null;
	}

	private List<TestBin> bins;
	private List<TestItem> items;
	private BinCollectionsTestDataProvider rundataProvider;

	//[Benchmark(Baseline = true)]
	//public FittingResult V1_5x5x5()
	//{
	//	var strategy = new Fitting.Algorithms.FirstFitDecreasing_v1()
	//		.WithBins(this.bins)
	//		.AndItems(this.items)
	//		.Build();

	//	var result = strategy.Execute();
	//	BenchmarkScalingTestsDataProvider.AssertSuccessfulResult(result, this.NoOfItems);
	//	return result;
	//}

	//[Benchmark]
	//public FittingResult V2_5x5x5()
	//{
	//	var strategy = new Fitting.Algorithms.FirstFitDecreasing_v2()
	//		.WithBins(this.bins)
	//		.AndItems(this.items)
	//		.Build();

	//	var result = strategy.Execute();
	//	BenchmarkScalingTestsDataProvider.AssertSuccessfulResult(result, this.NoOfItems);
	//	return result;
	//}

	[Benchmark]
	public PackingResult V3_5x5x5()
	{
		var expectedSize = BenchmarkScalingTestsDataProvider.TestCases[this.NoOfItems];
		var expectedBin = expectedSize != "None" ? this.bins.FirstOrDefault(x => x.ID == expectedSize) : this.bins.LastOrDefault(); 
		var algorithm = new Packing.Algorithms.FirstFitDecreasing_v1<TestBin, TestItem>(expectedBin, this.items);
		var result = algorithm.Execute();
		BenchmarkScalingTestsDataProvider.AssertSuccessfulResult(result, this.NoOfItems);
		return result;
	}
}
