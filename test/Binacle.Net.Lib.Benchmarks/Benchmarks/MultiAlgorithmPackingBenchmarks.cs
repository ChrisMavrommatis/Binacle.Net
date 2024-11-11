using BenchmarkDotNet.Attributes;
using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Packing.Models;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;
using System.Diagnostics.Contracts;

namespace Binacle.Net.Lib.Benchmarks;


[MemoryDiagnoser]
public class MultiAlgorithmPackingBenchmarks
{
	public MultiAlgorithmPackingBenchmarks()
	{
		this.binCollectionsDataProvider = new BinCollectionsTestDataProvider();

		// TODO Fix this
		this.scenario = BenchmarkScalingTestsDataProvider.Scenarios["Rectangular-Cuboids::Small"];
	}

	
	[ParamsSource(nameof(NoOfItemsParamsSourceAccessor))]
	public int NoOfItems { get; set; }

	[Params(2, 3, 4, 5, 6)]
	public int NoOfAlgorithms { get; set; }

	public IEnumerable<int> NoOfItemsParamsSourceAccessor()
	{
		return this.scenario.GetNoOfItems();
	}

	private BinCollectionsTestDataProvider binCollectionsDataProvider;
	private BenchmarkScalingScenario scenario;
	private TestBin? bin;
	private List<TestItem>? items;
	private readonly Func<TestBin, List<TestItem>, IPackingAlgorithm>[] algorithmFactories = [
		AlgorithmFactories.Packing_FFD_v1,
		AlgorithmFactories.Packing_FFD_v1,
		AlgorithmFactories.Packing_FFD_v1,
		AlgorithmFactories.Packing_FFD_v1,
		AlgorithmFactories.Packing_FFD_v1,
		AlgorithmFactories.Packing_FFD_v1,
		AlgorithmFactories.Packing_FFD_v1
	];

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

	[Benchmark(Baseline = true)]
	public PackingResult ForLoop()
	{
		var results = new PackingResult[NoOfAlgorithms];
		for(var i = 0; i< NoOfAlgorithms; i++)
		{
			var algorithmFactory = this.algorithmFactories[i];
			var algorithmInstance = algorithmFactory(this.bin!, this.items!);
			var result = algorithmInstance.Execute(new PackingParameters { OptInToEarlyFails = false, NeverReportUnpackedItems = false, ReportPackedItemsOnlyWhenFullyPacked = false });
			results[i] = result;
		}

		return results.OrderByDescending(x => x.PackedBinVolumePercentage).FirstOrDefault()!;
	}

	[Benchmark]
	public PackingResult ParallelFor()
	{
		var results = new PackingResult[NoOfAlgorithms];
		Parallel.For(0, NoOfAlgorithms, index =>
		{
			var algorithmFactory = this.algorithmFactories[index];
			var algorithmInstance = algorithmFactory(this.bin!, this.items!);
			var result = algorithmInstance.Execute(new PackingParameters { OptInToEarlyFails = false, NeverReportUnpackedItems = false, ReportPackedItemsOnlyWhenFullyPacked = false });
			results[index] = result;
		});
		return results.OrderByDescending(x => x.PackedBinVolumePercentage).FirstOrDefault()!;
	}


}
