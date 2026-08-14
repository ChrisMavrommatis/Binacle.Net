using Binacle.ViPaq.TestsKernel.Models;

namespace Binacle.ViPaq.TestsKernel.Providers;

// Real placed results for the custom, hand-authored problems: the bin plus the placed items the packer
// produced. Generated offline by Binacle.ViPaq.PackedDataGenerator (FFD, pinned), committed under
// vipaq/data/packed/custom-problems/ and read here as embedded resources. No token is stored - it is derivable,
// so the benchmark computes it. Do not hand-edit.
public static class CustomProblemsDataProvider
{
	private const string Family = "custom-problems";

	private static readonly Dictionary<string, Scenario> scenarios = new();

	static CustomProblemsDataProvider()
	{
		foreach (var scenario in PackedDataReader.Read(Family))
		{
			// See BischoffDataProvider: keyed by Name alone, so a second algorithm's `.bfd.json` in this folder
			// would collide on Add. Deferred until such data exists.
			scenarios.Add(scenario.Name, scenario);
		}
	}

	public static IReadOnlyCollection<Scenario> All => scenarios.Values;

	public static IEnumerable<string> Names => scenarios.Keys;

	public static Scenario GetByName(string name) => scenarios[name];
}
