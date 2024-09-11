using Binacle.Net.TestsKernel.Models;
using Binacle.Net.TestsKernel.Providers;
using System.Collections.ObjectModel;

namespace Binacle.Net.Lib.UnitTests;

public sealed class SanityFixture : IDisposable
{
	public BinCollectionsTestDataProvider BinCollectionsTestDataProvider { get; }
	public ScenarioCollectionsTestDataProvider ScenarioCollectionsTestDataProvider { get; }

	public SanityFixture()
	{
		BinCollectionsTestDataProvider = new BinCollectionsTestDataProvider(Data.Constants.SolutionRootBasePath);
		ScenarioCollectionsTestDataProvider = new ScenarioCollectionsTestDataProvider(Data.Constants.SolutionRootBasePath);
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
