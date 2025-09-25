using Binacle.Net.TestsKernel.Models;
using System.Collections;
using Binacle.Net.TestsKernel.Data.Providers;


namespace Binacle.Net.TestsKernel.Data.Providers;

public abstract class ScenarioDataProviderBase : IEnumerable<object[]>
{
	protected readonly ScenarioCollectionsDataProvider ScenarioCollections;
	protected readonly List<Scenario> Scenarios;

	protected ScenarioDataProviderBase(string collectionKey)
	{
		this.ScenarioCollections = new ScenarioCollectionsDataProvider();
		this.Scenarios = this.ScenarioCollections.GetCollection(collectionKey);
	}

	public virtual IEnumerator<object[]> GetEnumerator()
	{
		foreach (var scenario in this.Scenarios)
		{
			yield return new object[] { scenario };
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
