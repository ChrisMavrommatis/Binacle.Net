namespace Binacle.Lib.Benchmarks.Providers;

public static class BenchmarkScenariosProvider
{
	/*
		Scenario	BFD		FFD		WFD		Purpose
		thpack1_7	80.30%	78.08%	78.08%	Representative baseline
		thpack1_44	83.86%	62.65%	69.43%	BFD dominance (medium)
		thpack2_30	88.17%	87.75%	87.40%	High efficiency / low variance
		thpack2_35	85.86%	75.77%	56.82%	WFD weakness
		thpack7_56	84.65%	65.36%	60.74%	Hardest / max complexity
	*/
	
	
	public static Dictionary<string ,string> ScenarioDescriptions { get; }
		= new()
		{
			{ "Representative baseline", "OrLibrary_thpack1_7" },
			{ "BFD dominance (medium)", "OrLibrary_thpack1_44" },
			{ "High efficiency / low variance", "OrLibrary_thpack2_30"  },
			{ "WFD weakness" , "OrLibrary_thpack2_35" },
			{ "Hardest / max complexity", "OrLibrary_thpack7_56" },
		};
	
	public static string[] GetBenchmarkScenarios()
		=> ScenarioDescriptions.Keys.ToArray();

	public static string[] RepresentativeBaselineScenarios()
		=> [ScenarioDescriptions.Keys.First()];
}
