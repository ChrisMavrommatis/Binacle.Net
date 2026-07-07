using Binacle.ViPaq.TestsKernel.Models;
// [REVIEW-VIPAQ_TEST]
namespace Binacle.ViPaq.TestsKernel.Samples;

// What BenchmarkDotNet fans out over: a curated slice of synthetic samples plus a few real ones, so the
// encode/decode numbers cover both generated and real placed data without a benchmark run per catalog
// entry. Names resolve from whichever provider owns them.
public static class BenchmarkCatalog
{
	public static IEnumerable<string> Names =>
		SampleProvider.BenchmarkNames.Concat(RealDataProvider.BenchmarkNames);

	public static PackingSample GetByName(string name) =>
		RealDataProvider.Names.Contains(name)
			? RealDataProvider.GetByName(name)
			: SampleProvider.GetByName(name);
}
