using Binacle.ViPaq.TestsKernel.Models;

namespace Binacle.ViPaq.TestsKernel.Providers;

// The hand-picked Bischoff scenarios the benchmarks fan out over. Both are 16-bit real packs that clear the
// compression threshold, from two different thpack families, chosen to span the range deflate covers:
//
//   - OrLibrary_thpack4_1  — 70 items, raw 856 → deflate 396 b64 (saves ~54%). The lower end of the win: more
//     varied placement, so deflate has less to grip on.
//   - OrLibrary_thpack1_2  — 108 items, raw 1312 → deflate 404 b64 (saves ~69% row, ~77% columnar). The upper
//     end: a larger, more repetitive pack where compression and columnar pay the most.
//
// Names resolve through BischoffDataProvider, so a stale pick is caught by the curated check.
public static class BischoffCuratedProvider
{
	public static IEnumerable<string> Names =>
	[
		"OrLibrary_thpack4_1",
		"OrLibrary_thpack1_2"
	];

	public static Scenario GetByName(string name) => BischoffDataProvider.GetByName(name);
}
