using Binacle.TestsKernel.Models;
using Binacle.TestsKernel.Providers;

namespace Binacle.Lib.Benchmarks.Providers;

public static class BischoffSuiteScenarioProvider
{
	private static readonly Dictionary<string, Scenario> scenarios;
	static BischoffSuiteScenarioProvider()
	{
		var dataProvider = new BischoffSuiteDataProvider();
		scenarios = new Dictionary<string, Scenario>();
		foreach (var objectArray in dataProvider)
		{
			var scenario = (Scenario)objectArray[0]!;
			scenarios.Add(scenario.Name, scenario);
		}
	}

	public static IEnumerable<string> GetScenarioNames()
		=> scenarios.Keys.Take(2);

	public static IEnumerable<Scenario> GetScenarios()
		=> scenarios.Values;
	
	public static Scenario GetScenarioByName(string name)
		=> scenarios[name];
}
