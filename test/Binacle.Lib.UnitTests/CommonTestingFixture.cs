using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Net.TestsKernel.Data.Providers;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.UnitTests;

public sealed class CommonTestingFixture : IDisposable
{
	public BinCollectionsDataProvider BinDataProvider { get; }
	public Dictionary<string, AlgorithmFactory<IPackingAlgorithm>> AlgorithmsUnderTest { get; }

	public CommonTestingFixture()
	{
		this.BinDataProvider = new BinCollectionsDataProvider();
		this.AlgorithmsUnderTest = UnitTests.AlgorithmsUnderTest.All;
	}

	public List<TestBin> GetBins(string collectionKey)
	{
		return this.BinDataProvider.GetCollection(collectionKey);
	}

	public void Dispose()
	{

	}
}
