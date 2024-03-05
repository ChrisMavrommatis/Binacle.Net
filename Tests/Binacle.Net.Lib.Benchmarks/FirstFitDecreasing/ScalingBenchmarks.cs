using BenchmarkDotNet.Attributes;
using Binacle.Net.Lib.Tests.Data.Providers;
using Binacle.Net.Lib.Tests.Models;
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

		this.rundataProvider = new BinTestDataProvider(solutionRootBasePath: GetSolutionRoot());
		this.bins = this.rundataProvider.GetBinCollection(BenchmarkScalingTestsDataProvider.BinCollectionName);
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
	private BinTestDataProvider rundataProvider;

	[Benchmark(Baseline = true)]
	public Lib.Models.BinFittingOperationResult V1_5x5x5()
	{
		var strategy = new Strategies.FirstFitDecreasing_v1()
			.WithBins(this.bins)
			.AndItems(this.items)
			.Build();

		var result = strategy.Execute();
		BenchmarkScalingTestsDataProvider.AssertSuccessfulResult(result, this.NoOfItems);
		return result;
	}

	[Benchmark]
	public Lib.Models.BinFittingOperationResult V2_5x5x5()
	{
		var strategy = new Strategies.FirstFitDecreasing_v2()
			.WithBins(this.bins)
			.AndItems(this.items)
			.Build();

		var result = strategy.Execute();
		BenchmarkScalingTestsDataProvider.AssertSuccessfulResult(result, this.NoOfItems);
		return result;
	}
}
