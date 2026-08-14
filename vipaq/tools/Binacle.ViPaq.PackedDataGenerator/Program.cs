using Binacle.Lib;
using Binacle.TestReporting;

namespace Binacle.ViPaq.PackedDataGenerator;

// Packs the Bischoff suite and custom problems with Binacle.Lib and freezes the placed results as committed
// data files under vipaq/data/packed. Takes no arguments on purpose: a run always regenerates every algorithm
// in the list below, so it cannot half-run and leave the data mixed. Output is deterministic, so a no-change
// re-run is byte-identical.
//
// FFD is the pinned algorithm today. Adding WFD/BFD is one entry in the list; the algorithm rides on the file
// name as a ".<algo>" suffix (orlib_thpack1.ffd.json), so the sets sit side by side without mixing.
internal class Program
{
	// Where to read the problems, the subfolder to write placed results into, and the files.
	private static readonly SourceFamily[] Families =
	[
		new(
			InputDir: ["shared", "data", "custom-problems"],
			DestinationFolder: "custom-problems",
			Files: ["baseline.json", "complex.json", "simple.json"]),
		new(
			InputDir: ["shared", "data", "bischoff-suite"],
			DestinationFolder: "bischoff-suite",
			Files:
			[
				"orlib_thpack1.json", "orlib_thpack2.json", "orlib_thpack3.json", "orlib_thpack4.json",
				"orlib_thpack5.json", "orlib_thpack6.json", "orlib_thpack7.json",
			]),
	];

	// One generator per algorithm.
	private static readonly IReadOnlyList<PackedDataGenerator> Generators =
	[
		new PackedDataGenerator(Algorithm.FFD),
	];

	static async Task Main(string[] args)
	{
		var locator = RepositoryRoot.Bind();

		foreach (var generator in Generators)
		{
			var algorithm = generator.Algorithm;
			var suffix = algorithm.ToString().ToLowerInvariant();
			Console.WriteLine($"[{algorithm}] packing (.{suffix}.json)");

			var totalSamples = 0;
			var totalItems = 0;
			foreach (var family in Families)
			{
				var result = await generator.GenerateAsync(family, locator);
				totalSamples += result.Samples;
				totalItems += result.Items;
			}

			Console.WriteLine($"[{algorithm}] {totalSamples} samples, {totalItems} placed items.");
		}
	}
}
