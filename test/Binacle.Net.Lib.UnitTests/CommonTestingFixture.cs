using Binacle.Net.Lib.Abstractions.Algorithms;
using Binacle.Net.Lib.Abstractions.Fitting;
using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Net.Lib.UnitTests;

public sealed class CommonTestingFixture : IDisposable
{
	public BinCollectionsTestDataProvider BinTestDataProvider { get; }
	public AlgorithmFactory<IFittingAlgorithm>[] TestedFittingAlgorithms { get; }
	public AlgorithmFactory<IPackingAlgorithm>[] TestedPackingAlgorithms { get; }

	public CommonTestingFixture()
	{
		this.BinTestDataProvider = new BinCollectionsTestDataProvider();
		this.TestedFittingAlgorithms = [
			AlgorithmFactories.Fitting_FFD_v1,
			AlgorithmFactories.Fitting_FFD_v2,
			AlgorithmFactories.Fitting_FFD_v3
		];
		this.TestedPackingAlgorithms = [
			AlgorithmFactories.Packing_FFD_v1,
			AlgorithmFactories.Packing_FFD_v2,
			AlgorithmFactories.Packing_WFD_v1,
			AlgorithmFactories.Packing_BFD_v1
		];
	}

	public List<TestBin> GetBins(string collectionKey)
	{
		return BinTestDataProvider.GetCollection(collectionKey);
	}

	public void Dispose()
	{

	}
}
