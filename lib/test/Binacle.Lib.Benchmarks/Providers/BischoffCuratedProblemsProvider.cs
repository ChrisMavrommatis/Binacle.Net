namespace Binacle.Lib.Benchmarks.Providers;

public static class BischoffCuratedProblemsProvider
{
	// Five scenarios curated out of the full OR-library suite, chosen so each one stresses something
	// different. Numbers are the fill percentage each algorithm reaches on that scenario.
	//
	//   scenario     BFD      FFD      WFD     covers
	//   thpack1_7    80.30%   78.08%   78.08%  representative baseline
	//   thpack1_44   83.86%   62.65%   69.43%  BFD dominance (medium)
	//   thpack2_30   88.17%   87.75%   87.40%  high efficiency, low variance
	//   thpack2_35   85.86%   75.77%   56.82%  WFD weakness
	//   thpack7_56   84.65%   65.36%   60.74%  hardest, max complexity
	public static Dictionary<string ,string> ScenarioDescriptions { get; }
		= new()
		{
			{ "Baseline", "OrLibrary_thpack1_7" },
			{ "BFD dominance", "OrLibrary_thpack1_44" },
			{ "High efficiency", "OrLibrary_thpack2_30"  },
			{ "WFD weakness" , "OrLibrary_thpack2_35" },
			{ "Max complexity", "OrLibrary_thpack7_56" },
		};
	
	public static string[] GetBenchmarkScenarios()
		=> ScenarioDescriptions.Keys.ToArray();
}
