using Binacle.ViPaq.TestsKernel.Models;

namespace Binacle.ViPaq.TestsKernel.Providers;

// The hand-picked custom scenarios the benchmarks fan out over — the sibling of CustomProblemsDataProvider, which
// holds them all. Split by the two paths ViPaq takes so both are measured; the curated check in the performance
// tests confirms the uncompressed ones really stay uncompressed. Names resolve through CustomProblemsDataProvider.
public static class CustomProblemsCuratedProvider
{
	// Small packs that stay under the compression threshold — the raw, no-gzip path. Mostly 8-bit; the last is a
	// small 16-bit pack so the uncompressed 16-bit path (bin + item dimensions ≥ 256) is measured, not just 8-bit.
	public static IEnumerable<string> UncompressedNames =>
	[
		"Baseline_5x5x5-1_FitsIn_60x40x10",              // 1 item, 8-bit
		"Simple_15x15x15-8_FitIn_60x40x20",              // 8 items, 8-bit
		"Complex_FitsInMedium_1",                        // 16 items, 8-bit
		"Simple_16bit-4_FitIn_600x400x300"               // 4 items, 16-bit
	];

	// One dense pack that ViPaq compresses — the gzip path.
	public static IEnumerable<string> CompressedNames =>
	[
		"Simple_5x5x5-100_FitIn_60x40x10"     // 100 items
	];

	public static IEnumerable<string> Names => UncompressedNames.Concat(CompressedNames);

	public static Scenario GetByName(string name) => CustomProblemsDataProvider.GetByName(name);
}
