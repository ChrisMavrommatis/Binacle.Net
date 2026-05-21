using Binacle.TestsKernel.ResultSelection.Models;

namespace Binacle.TestsKernel.ResultSelection.Providers;

public static class AllScenariosProvider
{
    private static readonly Dictionary<string, Scenario> scenarios;
    static AllScenariosProvider()
    {
        MultipleScenarioCollectionsProvider[] dataProviders =
        [
            new MultipleScenarioCollectionsProvider(CollectionKeys.BestAlgorithm.ToArray()),
            new MultipleScenarioCollectionsProvider(CollectionKeys.BestBin.ToArray()),
            new MultipleScenarioCollectionsProvider(CollectionKeys.SmallestBin.ToArray()),
        ];
        scenarios = new Dictionary<string, Scenario>();
        foreach (var dataProvider in dataProviders)
        {
            foreach (var collectionScenario in dataProvider)
            {
                var scenario = collectionScenario.Scenario;
                scenarios.Add(scenario.Name, scenario);
            }
        }
    }

    public static IEnumerable<string> GetScenarioNames()
        => scenarios.Keys;
    
    public static IEnumerable<object[]> ScenarioNames
        => GetScenarioNames().Select(name => new object[] { name });

    public static IEnumerable<Scenario> GetScenarios()
        => scenarios.Values;
	
    public static Scenario GetScenarioByName(string name)
        => scenarios[name];
}
