using Binacle.ViPaq.TestsKernel.Models;

namespace Binacle.ViPaq.TestsKernel.Providers;

// The hand-picked Bischoff scenarios the benchmarks fan out over — the sibling of BischoffDataProvider, which
// holds them all. All are 16-bit and large enough that ViPaq compresses them, so they exercise the gzip path.
// Names resolve through BischoffDataProvider, so a stale pick is caught by the curated check in the performance
// tests.
public static class BischoffCuratedProvider
{
	public static IEnumerable<string> Names =>
	[
		"OrLibrary_thpack4_1",
		"OrLibrary_thpack1_2"
	];

	public static Scenario GetByName(string name) => BischoffDataProvider.GetByName(name);
}
