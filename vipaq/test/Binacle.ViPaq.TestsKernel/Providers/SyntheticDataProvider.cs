using Binacle.ViPaq.TestsKernel.Models;

namespace Binacle.ViPaq.TestsKernel.Providers;

// Synthetic (generated) scenarios — the sibling of the real BischoffDataProvider / CustomProblemsDataProvider.
// STUBBED: the generator and its matrix were dropped. Synthetic random data misreads compression for size, so it
// earned its keep only in the BDN speed/memory benchmarks; it's stubbed out for now, so the curated benchmark set
// is real data only. Rebuild this when synthetic speed/memory coverage is wanted again — it can scale to item
// counts (2000, 5000) that no real pack has.
public static class SyntheticDataProvider
{
	public static IReadOnlyCollection<Scenario> All => [];

	public static IEnumerable<string> Names => [];

	public static Scenario GetByName(string name) =>
		throw new KeyNotFoundException(
			$"Synthetic scenarios are stubbed; '{name}' is not available. Rebuild SyntheticDataProvider.");
}
