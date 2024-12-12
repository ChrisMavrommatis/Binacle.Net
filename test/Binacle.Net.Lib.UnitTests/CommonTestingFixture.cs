using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Lib.UnitTests;

public sealed class CommonTestingFixture : IDisposable
{
	public BinCollectionsTestDataProvider BinTestDataProvider { get; }
	public  Dictionary<string, AlgorithmFactory<IFittingAlgorithm>>  FittingAlgorithmsUnderTest { get; }
	public  Dictionary<string, AlgorithmFactory<IPackingAlgorithm>> PackingAlgorithmsUnderTest { get; }

	public CommonTestingFixture()
	{
		this.BinTestDataProvider = new BinCollectionsTestDataProvider();
		this.FittingAlgorithmsUnderTest = AlgorithmsUnderTest.FittingAlgorithms;
		this.PackingAlgorithmsUnderTest = AlgorithmsUnderTest.PackingAlgorithms;
	}

	public List<TestBin> GetBins(string collectionKey)
	{
		return BinTestDataProvider.GetCollection(collectionKey);
	}

	public void Dispose()
	{

	}
}
