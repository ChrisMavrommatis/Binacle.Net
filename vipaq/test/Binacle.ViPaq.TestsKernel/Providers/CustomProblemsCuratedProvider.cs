using Binacle.ViPaq.TestsKernel.Models;

namespace Binacle.ViPaq.TestsKernel.Providers;

// The hand-picked custom scenarios the benchmarks fan out over, split by the two paths ViPaq takes so both are
// measured. The curated check confirms the uncompressed ones really stay uncompressed.
public static class CustomProblemsCuratedProvider
{
	// Small packs under the compression threshold: the raw path. Mostly 8-bit; the last is a small 16-bit pack,
	// so the uncompressed 16-bit path is measured too.
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
