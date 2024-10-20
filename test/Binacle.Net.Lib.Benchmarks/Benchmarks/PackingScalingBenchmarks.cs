using BenchmarkDotNet.Attributes;
using Binacle.Net.Lib.Benchmarks.Order;
using Binacle.Net.Lib.Packing.Models;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;
using System.Runtime.CompilerServices;

namespace Binacle.Net.Lib.Benchmarks;


[MemoryDiagnoser]
public class PackingScalingBenchmarks
{
	public PackingScalingBenchmarks()
	{
		this.binCollectionsDataProvider = new BinCollectionsTestDataProvider();

		// TODO Fix this
		this.scenario = BenchmarkScalingTestsDataProvider.Scenarios["Rectangular-Cuboids::Small"];
	}

	[ParamsSource(nameof(NoOfItemsParamsSourceAccessor))]
	public int NoOfItems { get; set; }

	public IEnumerable<int> NoOfItemsParamsSourceAccessor()
	{
		return this.scenario.GetNoOfItems();
	}

	private BinCollectionsTestDataProvider binCollectionsDataProvider;
	private BenchmarkScalingScenario scenario;
	private TestBin? bin;
	private List<TestItem>? items;

	[GlobalSetup]
	public void GlobalSetup()
	{
		this.bin = this.scenario.GetTestBin(this.binCollectionsDataProvider);
		this.items = this.scenario.GetTestItems(this.NoOfItems);

	}

	[GlobalCleanup]
	public void GlobalCleanup()
	{
		this.bin = null;
		this.items = null;
	}

	#region FFD
	[Benchmark(Baseline = true)]
	[BenchmarkOrder(0)]
	[BenchmarkCategory("FFD")]
	public PackingResult Packing_FFD_V1()
	{
		var algorithmInstance = AlgorithmFactories.Packing_FFD_v1(this.bin!, this.items!);
		var result = algorithmInstance.Execute(new PackingParameters { OptInToEarlyFails = false, NeverReportUnpackedItems = false, ReportPackedItemsOnlyWhenFullyPacked = false });
		return result;
	}

	[Benchmark]
	[BenchmarkOrder(1)]
	[BenchmarkCategory("FFD")]
	public PackingResult Packing_FFD_V2()
	{
		var algorithmInstance = AlgorithmFactories.Packing_FFD_v2(this.bin!, this.items!);
		var result = algorithmInstance.Execute(new PackingParameters { OptInToEarlyFails = false, NeverReportUnpackedItems = false, ReportPackedItemsOnlyWhenFullyPacked = false });
		return result;
	}
	#endregion


	#region WFD
	[Benchmark]
	[BenchmarkOrder(3)]
	[BenchmarkCategory("WFD")]
	public PackingResult Packing_WFD_V1()
	{
		var algorithmInstance = AlgorithmFactories.Packing_WFD_v1(this.bin!, this.items!);
		var result = algorithmInstance.Execute(new PackingParameters { OptInToEarlyFails = false, NeverReportUnpackedItems = false, ReportPackedItemsOnlyWhenFullyPacked = false });
		return result;
	}
	#endregion


	#region BFD
	[Benchmark]
	[BenchmarkOrder(4)]
	[BenchmarkCategory("BFD")]
	public PackingResult Packing_BFD_V1()
	{
		var algorithmInstance = AlgorithmFactories.Packing_BFD_v1(this.bin!, this.items!);
		var result = algorithmInstance.Execute(new PackingParameters { OptInToEarlyFails = false, NeverReportUnpackedItems = false, ReportPackedItemsOnlyWhenFullyPacked = false });
		return result;
	}
	#endregion
}
