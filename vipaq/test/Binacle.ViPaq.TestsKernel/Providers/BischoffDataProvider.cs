using Binacle.ViPaq.TestsKernel.Models;

namespace Binacle.ViPaq.TestsKernel.Providers;

// Real placed results for the Bischoff suite (thpack1..7): each scenario is the bin plus the *placed* items the
// packer produced (L,W,H and X,Y,Z). Generated offline by Binacle.ViPaq.PackedDataGenerator (FFD, pinned) and
// committed under vipaq/data/packed/bischoff-suite/; read here as embedded resources at static init. Only placed
// geometry is stored (no token) — the token is derivable, so we compute it when we benchmark. Do not hand-edit.
public static class BischoffDataProvider
{
	private const string Family = "bischoff-suite";

	private static readonly Dictionary<string, Scenario> scenarios = new();

	static BischoffDataProvider()
	{
		foreach (var scenario in PackedDataReader.Read(Family))
		{
			scenarios.Add(scenario.Name, scenario);
		}
	}

	public static IReadOnlyCollection<Scenario> All => scenarios.Values;

	public static IEnumerable<string> Names => scenarios.Keys;

	public static Scenario GetByName(string name) => scenarios[name];
}
