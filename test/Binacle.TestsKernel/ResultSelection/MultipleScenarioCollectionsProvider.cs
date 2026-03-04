using System.Collections;

namespace Binacle.TestsKernel.ResultSelection;

internal class MultipleScenarioCollectionsProvider : IEnumerable<Models.CollectionScenario>
{
    private readonly List<Models.CollectionScenario> scenarios;
    internal MultipleScenarioCollectionsProvider(string[] collectionKeys)
    {
        this.scenarios = new List<Models.CollectionScenario>();
        foreach (var collectionKey in collectionKeys)
        {
            var collectionScenarios = ScenarioCollectionsProvider.GetScenarios(collectionKey)
                .Select(x => new Models.CollectionScenario(collectionKey, x));
            this.scenarios.AddRange(collectionScenarios);
        }
    }
    public virtual IEnumerator<Models.CollectionScenario> GetEnumerator()
    {
        foreach (var scenario in this.scenarios)
        {
            yield return scenario;
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
