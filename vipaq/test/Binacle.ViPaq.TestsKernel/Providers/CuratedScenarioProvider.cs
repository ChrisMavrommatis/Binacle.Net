using Binacle.ViPaq.TestsKernel.Models;

namespace Binacle.ViPaq.TestsKernel.Providers;

// The full curated set the benchmarks fan out over — both families merged into one lookup. BischoffCuratedProvider
// and CustomProblemsCuratedProvider pick the scenarios per family; this joins them and resolves each by name.
public static class CuratedScenarioProvider
{
	private static readonly Dictionary<string, Scenario> scenarios;

	static CuratedScenarioProvider()
	{
		scenarios = new Dictionary<string, Scenario>();

		foreach (var name in BischoffCuratedProvider.Names)
		{
			scenarios[name] = BischoffCuratedProvider.GetByName(name);
		}

		foreach (var name in CustomProblemsCuratedProvider.Names)
		{
			scenarios[name] = CustomProblemsCuratedProvider.GetByName(name);
		}
	}

	public static IEnumerable<string> GetScenarioNames() => scenarios.Keys;

	public static Scenario GetScenarioByName(string name) => scenarios[name];
}
