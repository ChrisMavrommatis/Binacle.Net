using Binacle.Net.TestsKernel.Models;
using System.Collections;


namespace Binacle.Net.TestsKernel.Data.Providers;

public class MultipleCollectionScenarioDataProvider : IEnumerable<object[]>
{
	private readonly ScenarioCollectionsDataProvider scenarioCollections;
	private readonly List<Scenario> scenarios;
	protected MultipleCollectionScenarioDataProvider(string[] collectionKeys)
	{
		this.scenarioCollections = new ScenarioCollectionsDataProvider();
		this.scenarios = new List<Scenario>();
		foreach (var collectionKey in collectionKeys)
		{
			this.scenarios.AddRange(this.scenarioCollections.GetCollection(collectionKey));
		}
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
