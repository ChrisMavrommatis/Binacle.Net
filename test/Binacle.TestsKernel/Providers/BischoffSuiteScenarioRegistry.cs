using System.ComponentModel;
using Binacle.TestsKernel.Models;

namespace Binacle.TestsKernel.Providers;


public static class BischoffSuiteScenarioRegistry
{
	private static readonly Dictionary<string, Scenario> scenarios;
	static BischoffSuiteScenarioRegistry()
	{
		var dataProvider = new BischoffSuiteProvider();
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

public static class AllScenariosRegistry
{
	private static readonly Dictionary<string, Scenario> scenarios;
	static AllScenariosRegistry()
	{
		MultipleScenarioCollectionsProvider[] dataProviders =
		[
			new BischoffSuiteProvider(),
			new CustomProblemsProvider()
		];
		scenarios = new Dictionary<string, Scenario>();
		foreach (var dataProvider in dataProviders)
		{
			foreach (var objectArray in dataProvider)
			{
				var scenario = (Scenario)objectArray[0]!;
				scenarios.Add(scenario.Name, scenario);
			}
		}
	}

	public static IEnumerable<string> GetScenarioNames()
		=> scenarios.Keys;

	public static IEnumerable<Scenario> GetScenarios()
		=> scenarios.Values;
	
	public static Scenario GetScenarioByName(string name)
		=> scenarios[name];
}
