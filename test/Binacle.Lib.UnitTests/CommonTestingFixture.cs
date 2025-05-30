using Binacle.Lib.Abstractions.Algorithms;
using Binacle.Lib.Abstractions.Fitting;
using Binacle.Net.TestsKernel.Data.Providers;
using Binacle.Net.TestsKernel.Models;

namespace Binacle.Lib.UnitTests;

public sealed class CommonTestingFixture : IDisposable
{
	public BinCollectionsDataProvider BinDataProvider { get; }
	public Dictionary<string, AlgorithmFactory<IFittingAlgorithm>>  FittingAlgorithmsUnderTest { get; }
	public Dictionary<string, AlgorithmFactory<IPackingAlgorithm>> PackingAlgorithmsUnderTest { get; }

	public CommonTestingFixture()
	{
		this.BinDataProvider = new BinCollectionsDataProvider();
		this.FittingAlgorithmsUnderTest = AlgorithmsUnderTest.FittingAlgorithms;
		this.PackingAlgorithmsUnderTest = AlgorithmsUnderTest.PackingAlgorithms;
	}

	public List<TestBin> GetBins(string collectionKey)
	{
		return this.BinDataProvider.GetCollection(collectionKey);
	}

	public void Dispose()
	{

	}
}
