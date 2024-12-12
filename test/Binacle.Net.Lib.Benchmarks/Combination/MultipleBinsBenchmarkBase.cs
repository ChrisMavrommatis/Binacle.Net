using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Lib.Benchmarks.Combination;

public abstract class MultipleBinsBenchmarkBase
{
	protected readonly BinCollectionsTestDataProvider BinCollectionsDataProvider;
	protected readonly MultiBinsBenchmarkTestsDataProvider DataProvider;
	public MultipleBinsBenchmarkBase()
	{
		this.BinCollectionsDataProvider = new BinCollectionsTestDataProvider();
		this.DataProvider = new MultiBinsBenchmarkTestsDataProvider();
		
	}
	
}
