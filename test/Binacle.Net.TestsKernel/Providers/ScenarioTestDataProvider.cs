using Binacle.Net.TestsKernel.Models;
using System.Collections;


namespace Binacle.Net.TestsKernel.Providers;

public abstract class ScenarioTestDataProvider : IEnumerable<object[]>
{
	private readonly ScenarioCollectionsTestDataProvider scenarioCollections;
	private readonly List<Scenario> scenarios;

	protected ScenarioTestDataProvider(string collectionKey)
	{
		this.scenarioCollections = new ScenarioCollectionsTestDataProvider();
		this.scenarios = this.scenarioCollections.GetCollection(collectionKey);
	}

	public virtual IEnumerator<object[]> GetEnumerator()
	{
		foreach (var scenario in this.scenarios)
		{
			yield return new object[] { scenario };
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
