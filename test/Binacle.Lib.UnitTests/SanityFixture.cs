using Binacle.Net.TestsKernel.Data.Providers;
using Binacle.TestsKernel.Models;

namespace Binacle.Lib.UnitTests;

public sealed class SanityFixture : IDisposable
{
	public BinCollectionsDataProvider BinCollectionsDataProvider { get; }
	public ScenarioCollectionsDataProvider ScenarioCollectionsDataProvider { get; }

	public SanityFixture()
	{
		this.BinCollectionsDataProvider = new BinCollectionsDataProvider();
		this.ScenarioCollectionsDataProvider = new ScenarioCollectionsDataProvider();
	}

	public List<TestBin> GetBins(string collectionKey)
	{
		return this.BinCollectionsDataProvider.GetCollection(collectionKey);
	}

	public List<Scenario> GetScenarios(string collectionKey)
	{
		return this.ScenarioCollectionsDataProvider.GetCollection(collectionKey);
	}
	public void Dispose()
	{

	}
}
