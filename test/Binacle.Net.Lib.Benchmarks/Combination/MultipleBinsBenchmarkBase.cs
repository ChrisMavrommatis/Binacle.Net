using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Lib.Benchmarks.Combination;

public abstract class MultipleBinsBenchmarkBase
{
	protected readonly BinCollectionsTestDataProvider BinCollectionsDataProvider;
	protected  readonly MultiBinsBenchmarkTestsDataProvider DataProvider;
	protected  readonly List<TestBin> Bins;
	protected  readonly List<TestItem> Items;
	public MultipleBinsBenchmarkBase()
	{
		this.BinCollectionsDataProvider = new BinCollectionsTestDataProvider();
		this.DataProvider = new MultiBinsBenchmarkTestsDataProvider();
		this.Bins = this.DataProvider.GetBins(this.BinCollectionsDataProvider);
		this.Items = this.DataProvider.GetItems();
	}
	
}
