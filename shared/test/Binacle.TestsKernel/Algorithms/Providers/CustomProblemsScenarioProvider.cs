using Binacle.TestsKernel.Algorithms.Models;
using Binacle.TestsKernel.Models;

namespace Binacle.TestsKernel.Algorithms.Providers;


public static class CustomProblemsScenarioProvider
{
	private static readonly Dictionary<string, Scenario> scenarios;
	private static readonly List<TestBin> distinctBins;

	static CustomProblemsScenarioProvider()
	{
		var dataProvider = new MultipleScenarioCollectionsProvider(CollectionKeys.CustomProblems.ToArray());
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

	// The bins these scenarios run against, in the order the scenarios introduce them. The API test host
	// registers exactly this set as the `custom-problems` preset, so the list is never written down twice.
	public static IReadOnlyList<TestBin> GetDistinctBins()
		=> distinctBins;

	public static IEnumerable<string> GetDistinctBinIds()
		=> distinctBins.Select(x => x.ID);

	// An item fitting this bin fits every one of them, so a caller can name the expected winner of a
	// smallest-bin or best-fit selection without hardcoding an ID.
	public static TestBin GetSmallestBin()
		=> distinctBins.MinBy(bin => bin.CalculateVolume())!;
}
