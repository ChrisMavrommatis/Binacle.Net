using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;

namespace Binacle.Lib.UnitTests;

public sealed class SanityFixture : IDisposable
{
	public BinCollectionsTestDataProvider BinCollectionsTestDataProvider { get; }
	public ScenarioCollectionsTestDataProvider ScenarioCollectionsTestDataProvider { get; }

	public SanityFixture()
	{
		BinCollectionsTestDataProvider = new BinCollectionsTestDataProvider();
		ScenarioCollectionsTestDataProvider = new ScenarioCollectionsTestDataProvider();
	}

	public List<TestBin> GetBins(string collectionKey)
	{
		return BinCollectionsTestDataProvider.GetCollection(collectionKey);
	}

	public List<Scenario> GetScenarios(string collectionKey)
	{
		return ScenarioCollectionsTestDataProvider.GetCollection(collectionKey);
	}
	public void Dispose()
	{

	}
}
