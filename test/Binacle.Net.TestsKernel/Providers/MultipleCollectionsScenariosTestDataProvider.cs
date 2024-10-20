using Binacle.Net.TestsKernel.Models;
using System.Collections;


namespace Binacle.Net.TestsKernel.Providers;

public class MultipleCollectionScenarioTestDataProvider : IEnumerable<object[]>
{
	private readonly ScenarioCollectionsTestDataProvider scenarioCollections;
	private readonly List<Scenario> scenarios;
	protected MultipleCollectionScenarioTestDataProvider(string[] collectionKeys)
	{
		this.scenarioCollections = new ScenarioCollectionsTestDataProvider();
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
