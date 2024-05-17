using Binacle.Net.TestsKernel.Models;
using System.Collections;


namespace Binacle.Net.TestsKernel.Providers;

public class ScenarioTestDataProvider : IEnumerable<object[]>
{
	private readonly CompactScenarioCollectionsTestDataProvider scenarioCollections;
	private readonly List<Scenario> scenarios;

	protected ScenarioTestDataProvider(string solutionRootBasePath, string scenarioCollectionName)
	{
		this.scenarioCollections = new CompactScenarioCollectionsTestDataProvider(solutionRootBasePath);
		this.scenarios = this.scenarioCollections.GetCollection(scenarioCollectionName);
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
