using Binacle.ViPaq.TestsKernel.Models;

namespace Binacle.ViPaq.TestsKernel.Providers;

// The hand-picked Bischoff scenarios the benchmarks fan out over — the sibling of BischoffDataProvider, which
// holds them all. Both are 16-bit real packs that clear the compression threshold, chosen from the size report
// (results/vipaq/compression/) to span the range deflate covers, and from two different thpack families:
//
//   - OrLibrary_thpack4_1  — 70 items, raw 856 → deflate 396 b64 (saves ~54%). The lower end of the win: more
//     varied placement, so deflate has less to grip on.
//   - OrLibrary_thpack1_2  — 108 items, raw 1312 → deflate 404 b64 (saves ~69% row, ~77% columnar). The upper
//     end: a larger, more repetitive pack where compression — and columnar — pay the most.
//
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
