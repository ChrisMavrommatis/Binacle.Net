using Binacle.Net.TestsKernel.Models;
using System.Collections;


namespace Binacle.Net.TestsKernel.Providers;

public abstract class ScenarioTestDataProvider : IEnumerable<object[]>
{
	protected readonly ScenarioCollectionsTestDataProvider ScenarioCollections;
	protected readonly List<Scenario> Scenarios;

	protected ScenarioTestDataProvider(string collectionKey)
	{
		this.ScenarioCollections = new ScenarioCollectionsTestDataProvider();
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
