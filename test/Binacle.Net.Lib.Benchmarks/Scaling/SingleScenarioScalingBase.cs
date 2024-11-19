using BenchmarkDotNet.Attributes;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Lib.Benchmarks.Scaling;

public abstract class SingleScenarioScalingBase
{
	public SingleScenarioScalingBase(string scenarioName)
	{
		this.binCollectionsDataProvider = new BinCollectionsTestDataProvider();
		this.scenario = ScalingBenchmarkTestsDataProvider.Scenarios[scenarioName];
	}

	[ParamsSource(nameof(NoOfItemsParamsSourceAccessor))]
	public int NoOfItems { get; set; }

	public IEnumerable<int> NoOfItemsParamsSourceAccessor()
	{
		return this.scenario.GetNoOfItems();
	}

	private BinCollectionsTestDataProvider binCollectionsDataProvider;
	private ScalingBenchmarkScenario scenario;
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
}
