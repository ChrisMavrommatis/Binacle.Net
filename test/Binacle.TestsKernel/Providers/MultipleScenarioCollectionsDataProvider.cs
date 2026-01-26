using System.Collections;
using Binacle.TestsKernel.Models;

namespace Binacle.TestsKernel.Providers;

public class MultipleScenarioCollectionsDataProvider : IEnumerable<object[]>
{
	private readonly List<Scenario> scenarios;
	protected MultipleScenarioCollectionsDataProvider(string[] collectionKeys)
	{
		var scenarioCollections = new ScenarioCollectionsDataProvider();
		this.scenarios = new List<Scenario>();
		foreach (var collectionKey in collectionKeys)
		{
			this.scenarios.AddRange(scenarioCollections.GetScenarios(collectionKey));
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
