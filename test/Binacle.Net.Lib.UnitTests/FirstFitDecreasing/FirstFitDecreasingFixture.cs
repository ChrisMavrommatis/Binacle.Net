using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Lib.UnitTests.FirstFitDecreasing;

public sealed class FirstFitDecreasingFixture : IDisposable
{
	private readonly BinCollectionsTestDataProvider binTestDataProvider;

	public Dictionary<string, List<TestBin>> Bins => this.binTestDataProvider.Collections;

	public FirstFitDecreasingFixture()
	{
		this.binTestDataProvider = new BinCollectionsTestDataProvider(Data.Constants.SolutionRootBasePath);

	}

	public void Dispose()
	{

	}

}
