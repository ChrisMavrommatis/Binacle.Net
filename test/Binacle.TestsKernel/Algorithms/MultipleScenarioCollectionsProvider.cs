using System.Collections;
using Binacle.TestsKernel.Algorithms.Models;

namespace Binacle.TestsKernel.Algorithms;

internal class MultipleScenarioCollectionsProvider : IEnumerable<CollectionScenario>
{
	private readonly List<CollectionScenario> scenarios;
	internal MultipleScenarioCollectionsProvider(string[] collectionKeys)
	{
		this.scenarios = new List<CollectionScenario>();
		foreach (var collectionKey in collectionKeys)
		{
			var collectionScenarios = ScenarioCollectionsProvider.GetScenarios(collectionKey)
				.Select(x => new CollectionScenario(collectionKey, x));
			this.scenarios.AddRange(collectionScenarios);
		}
	}
	public virtual IEnumerator<CollectionScenario> GetEnumerator()
	{
		foreach (var scenario in this.scenarios)
		{
			yield return scenario;
		}
	}
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
