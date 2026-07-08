using Binacle.ViPaq.TestsKernel.Models;
// [REVIEW-VIPAQ_TEST]
namespace Binacle.ViPaq.TestsKernel.Samples;

// What BenchmarkDotNet fans out over: a curated slice from each provider, so the encode/decode numbers cover
// the sample sets without a benchmark run per catalog entry. Names resolve from whichever provider owns them.
// SyntheticDataProvider is currently stubbed (returns nothing), so this is real data only until it's rebuilt.
public static class BenchmarkCatalog
{
	public static IEnumerable<string> Names =>
		SyntheticDataProvider.BenchmarkNames.Concat(RealDataProvider.BenchmarkNames);

	public static PackingSample GetByName(string name) =>
		RealDataProvider.Names.Contains(name)
			? RealDataProvider.GetByName(name)
			: SyntheticDataProvider.GetByName(name);
}
