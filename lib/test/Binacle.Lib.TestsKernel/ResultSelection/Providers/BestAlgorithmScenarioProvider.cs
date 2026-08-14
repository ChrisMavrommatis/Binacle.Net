using Binacle.Lib.TestsKernel.ResultSelection.Models;

namespace Binacle.Lib.TestsKernel.ResultSelection.Providers;

public static class BestAlgorithmScenarioProvider
{
    private static readonly Dictionary<string, Scenario> scenarios;
    static BestAlgorithmScenarioProvider()
    {
        var dataProvider = new MultipleScenarioCollectionsProvider(CollectionKeys.BestAlgorithm.ToArray());
        scenarios = new Dictionary<string, Scenario>();
        foreach (var collectionScenario in dataProvider)
        {
            var scenario = collectionScenario.Scenario;
            scenarios.Add(scenario.Name, scenario);
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
