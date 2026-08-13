using Binacle.ViPaq.TestsKernel.Models;

namespace Binacle.ViPaq.TestsKernel.Providers;

// Real placed results for the Bischoff suite (thpack1..7): the bin plus the placed items the packer produced.
// Generated offline by Binacle.ViPaq.PackedDataGenerator (FFD, pinned), committed under
// vipaq/data/packed/bischoff-suite/ and read here as embedded resources. No token is stored - it is derivable,
// so the benchmark computes it. Do not hand-edit.
public static class BischoffDataProvider
{
	private const string Family = "bischoff-suite";

	private static readonly Dictionary<string, Scenario> scenarios = new();

	static BischoffDataProvider()
	{
		foreach (var scenario in PackedDataReader.Read(Family))
		{
			// Keyed by Name alone. Every file is `.ffd` today, so names are unique; a second algorithm's
			// `.bfd.json` in the same folder would carry the same names and this Add would throw. Deferred until
			// such data exists.
			scenarios.Add(scenario.Name, scenario);
		}
	}

	public static IReadOnlyCollection<Scenario> All => scenarios.Values;

	public static IEnumerable<string> Names => scenarios.Keys;

	public static Scenario GetByName(string name) => scenarios[name];
}
