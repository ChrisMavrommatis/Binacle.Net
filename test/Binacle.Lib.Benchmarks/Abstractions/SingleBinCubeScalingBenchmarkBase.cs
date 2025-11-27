using BenchmarkDotNet.Attributes;
using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Fitting;
using Binacle.Lib.Fitting.Models;
using Binacle.Lib.Packing.Models;
using Binacle.Net.TestsKernel.Data.Providers;
using Binacle.Net.TestsKernel.Data.Providers.Benchmarks;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.Benchmarks.Abstractions;

public abstract class SingleBinCubeScalingBenchmarkBase
{
	public SingleBinCubeScalingBenchmarkBase(string scenarioName)
	{
		this.binCollectionsDataProvider = new BinCollectionsDataProvider();
		this.scenario = CubeScalingBenchmarksDataProvider.Scenarios[scenarioName];
	}

	[ParamsSource(nameof(NoOfItemsParamsSourceAccessor))]
	public int NoOfItems { get; set; }

	public IEnumerable<int> NoOfItemsParamsSourceAccessor()
	{
		return this.scenario.GetNoOfItems();
	}

	private BinCollectionsDataProvider binCollectionsDataProvider;
	private CubeScalingBenchmarkScenario scenario;
	protected TestBin? Bin { get; private set; }
	protected List<TestItem>? Items { get; private set; }

	[GlobalSetup]
	public void GlobalSetup()
	{
		this.Bin = this.scenario.GetTestBin(this.binCollectionsDataProvider);
		this.Items = this.scenario.GetTestItems(this.NoOfItems);
	}

	[GlobalCleanup]
	public void GlobalCleanup()
	{
		this.Bin = null;
		this.Items = null;
	}
	
	protected FittingResult Run(AlgorithmFactory<IFittingAlgorithm> algorithmFactory, TestBin bin, List<TestItem> items)
	{
		var algorithmInstance = algorithmFactory(bin, items);
		var result = algorithmInstance.Execute(new FittingParameters
		{
			ReportFittedItems = false,
			ReportUnfittedItems = false
		});
		return result;
	}
	
	protected PackingResult Run(
		AlgorithmFactory<IPackingAlgorithm> algorithmFactory,
		TestBin bin,
		List<TestItem> items
	)
	{
		var algorithmInstance = algorithmFactory(bin, items);
		var result = algorithmInstance.Execute(new PackingParameters
		{
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false,
			OptInToEarlyFails = true
		});
		return result;
	}
}
