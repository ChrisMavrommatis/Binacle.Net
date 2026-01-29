using System.Collections;
using Binacle.TestsKernel.Models;

namespace Binacle.TestsKernel.Providers;

public class MultipleScenarioCollectionsProvider : IEnumerable<object[]>
{
	private readonly List<Scenario> scenarios;
	protected MultipleScenarioCollectionsProvider(string[] collectionKeys)
	{
		var scenarioCollections = new ScenarioCollectionsProvider();
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
