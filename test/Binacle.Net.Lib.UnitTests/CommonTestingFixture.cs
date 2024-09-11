using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Lib.UnitTests;

public sealed class CommonTestingFixture : IDisposable
{
	public BinCollectionsTestDataProvider BinTestDataProvider { get; }

	public CommonTestingFixture()
	{
		BinTestDataProvider = new BinCollectionsTestDataProvider(Data.Constants.SolutionRootBasePath);

	}

	public List<TestBin> GetBins(string collectionKey)
	{
		return BinTestDataProvider.GetCollection(collectionKey);
	}

	public void Dispose()
	{

	}
}
