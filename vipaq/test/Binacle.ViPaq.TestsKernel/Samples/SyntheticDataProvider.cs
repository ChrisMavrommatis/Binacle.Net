using Binacle.ViPaq.TestsKernel.Models;
// [REVIEW-VIPAQ_TEST]
namespace Binacle.ViPaq.TestsKernel.Samples;

// Synthetic (generated) benchmark samples — the sibling of RealDataProvider. STUBBED: the generator and its
// matrix were dropped. Synthetic random data misreads compression for size, so it earned its keep only in the
// BDN speed/memory benchmarks; it's stubbed out for now and BenchmarkCatalog runs on real data only. Rebuild
// this when synthetic speed/memory coverage is wanted again — it can scale to item counts (2000, 5000) that no
// real pack has.
public static class SyntheticDataProvider
{
	public static IReadOnlyCollection<PackingSample> All => [];

	public static IEnumerable<string> Names => [];

	public static IEnumerable<string> BenchmarkNames => [];

	public static PackingSample GetByName(string name) =>
		throw new KeyNotFoundException(
			$"Synthetic samples are stubbed; '{name}' is not available. Rebuild SyntheticDataProvider.");
}
