using Binacle.TestsKernel.Algorithms.Models;
using Binacle.TestsKernel.Models;

namespace Binacle.TestsKernel.Algorithms.Providers;

public static class BischoffSuiteScenarioProvider
{
	private static readonly Dictionary<string, Scenario> scenarios;
	private static readonly List<TestBin> distinctBins;

	static BischoffSuiteScenarioProvider()
	{
		var dataProvider = new MultipleScenarioCollectionsProvider(CollectionKeys.BischoffSuite.ToArray());
		scenarios = new Dictionary<string, Scenario>();
		foreach (var collectionScenario in dataProvider)
		{
			var scenario = collectionScenario.Scenario;
			scenarios.Add(scenario.Name, scenario);
		}

		distinctBins = scenarios.Values
			.Select(x => x.Bin)
			.DistinctBy(x => x.ID)
			.ToList();
	}

	public static IEnumerable<string> GetScenarioNames()
		=> scenarios.Keys;

	public static IEnumerable<object[]> ScenarioNames
		=> GetScenarioNames().Select(name => new object[] { name });


	public static IEnumerable<Scenario> GetScenarios()
		=> scenarios.Values;

	public static Scenario GetScenarioByName(string name)
		=> scenarios[name];

	// The bins these scenarios run against, one entry per ID. The API test host registers exactly this set as the
	// `biscoff-suite` preset — see the note on CustomProblemsScenarioProvider.GetDistinctBins.
	public static IReadOnlyList<TestBin> GetDistinctBins()
		=> distinctBins;
}
