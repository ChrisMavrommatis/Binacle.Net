using Binacle.TestsKernel.Models;

namespace Binacle.TestsKernel.Providers;

public static class CustomProblemsScenarioRegistry
{
	private static readonly Dictionary<string, Scenario> scenarios;
	static CustomProblemsScenarioRegistry()
	{
		var dataProvider = new CustomProblemsProvider();
		scenarios = new Dictionary<string, Scenario>();
		foreach (var objectArray in dataProvider)
		{
			var scenario = (Scenario)objectArray[0]!;
			scenarios.Add(scenario.Name, scenario);
		}
	}

	public static IEnumerable<string> GetScenarioNames()
		=> scenarios.Keys;

	public static IEnumerable<Scenario> GetScenarios()
		=> scenarios.Values;
	
	public static Scenario GetScenarioByName(string name)
		=> scenarios[name];
}
