using System.Text.Encodings.Web;
using System.Text.Json;
using Binacle.CompactNotation;
using Binacle.Geometry;
using Binacle.Lib;
using Binacle.Lib.Abstractions.Models;
using Binacle.TestReporting;
using LibModels = Binacle.Lib.Models;

namespace Binacle.ViPaq.PackedDataGenerator;

// Packs every source problem with one algorithm and writes the placed results (one file per source problem,
// into the bischoff-suite/ and custom-problems/ subfolders of vipaq/data/packed). The algorithm rides on the
// file name as a ".<algo>" suffix, so a second algorithm sits beside this one in the same folders. Packing
// reproduces the API's exact call path — AlgorithmFactory.Create(...).Execute(Packing). Every sample is
// round-tripped (encode then decode == input) before it counts as good; Generate returns false if any fails.
public sealed class PackedDataGenerator
{
	private static readonly JsonSerializerOptions ReadOptions = new()
	{
		// Source keys and POCO properties are both PascalCase, so no case-insensitive matching is needed.
		PropertyNameCaseInsensitive = false,
		ReadCommentHandling = JsonCommentHandling.Skip,
	};

	private static readonly JsonSerializerOptions WriteOptions = new()
	{
		WriteIndented = true,
		IndentCharacter = '\t',
		IndentSize = 1,
		// Keep coordinate commas and 'x' literal instead of \u escapes — the content is plain text, so this is
		// safe and the committed files stay readable.
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
	};

	// Where placed results are written, as path segments under the repo root. Each family gets its own subfolder.
	private static readonly string[] OutputRoot = ["vipaq", "data", "packed"];

	// The source families. For each: where to read the problems (InputDir), the subfolder to write placed results
	// into (DestinationFolder), and its files. All directory knowledge lives here — Generate resolves these
	// against the repo root and does no ad-hoc path building.
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

	public bool Generate(Algorithm algorithm)
	{
		var suffix = algorithm.ToString().ToLowerInvariant();
		var repo = RepositoryRoot.Bind();
		var factory = new AlgorithmFactory();

		Console.WriteLine($"[{algorithm}] packing (.{suffix}.json)");

		var totalSamples = 0;
		var totalItems = 0;
		var allRoundTripped = true;

		foreach (var family in Families)
		{
			var inputDir = repo.Find(family.InputDir);
			var destinationDir = repo.Find([.. OutputRoot, family.DestinationFolder]);
			Directory.CreateDirectory(destinationDir);

			foreach (var file in family.Files)
			{
				var scenarios = JsonSerializer.Deserialize<SourceScenario[]>(
					File.ReadAllText(Path.Combine(inputDir, file)), ReadOptions)
					?? throw new InvalidOperationException($"{file} deserialized to null.");

				var samples = new List<PackedSample>(scenarios.Length);
				var fileItems = 0;
				foreach (var scenario in scenarios)
				{
					var (sample, roundTripped) = this.PackScenario(algorithm, factory, scenario);
					samples.Add(sample);
					fileItems += sample.Items.Length;
					if (!roundTripped)
					{
						allRoundTripped = false;
					}
				}

				var outputFile = $"{Path.GetFileNameWithoutExtension(file)}.{suffix}.json";
				File.WriteAllText(
					Path.Combine(destinationDir, outputFile), JsonSerializer.Serialize(samples, WriteOptions) + "\n");

				totalSamples += samples.Count;
				totalItems += fileItems;
				Console.WriteLine($"  {family.DestinationFolder}/{outputFile}: {samples.Count} samples, {fileItems} placed items");
			}
		}

		Console.WriteLine(
			$"[{algorithm}] {totalSamples} samples, {totalItems} placed items. Round-trip: {(allRoundTripped ? "OK" : "FAILED")}");
		return allRoundTripped;
	}

	private (PackedSample Sample, bool RoundTripped) PackScenario(
		Algorithm algorithm, AlgorithmFactory factory, SourceScenario scenario)
	{
		var binDims = CompactNotationParser.ParseDimensions<int>(scenario.Bin);
		var bin = new LibModels.Bin("bin", binDims.Length, binDims.Width, binDims.Height);

		var items = new List<LibModels.Item>();
		var index = 0;
		foreach (var itemString in scenario.Items)
		{
			// Item *types* with a quantity — pass the quantity to the lib Item; the algorithm expands it.
			var itemType = CompactNotationParser.ParseDimensionsAndQuantity<int>(itemString);
			items.Add(new LibModels.Item(
				$"item-{index++}", itemType.Length, itemType.Width, itemType.Height, itemType.Quantity));
		}

		var result = factory.Create(algorithm, bin, items).Execute(new PackingOperationParameters());

		// Placed items as int-based geometry — what CompactNotationFormatter and ViPaqSerializer.SerializeInt32 read.
		var placed = result.PackedItems
			.Select(p => new Item<int>
			{
				Length = p.Length, Width = p.Width, Height = p.Height,
				X = p.X, Y = p.Y, Z = p.Z,
			})
			.ToList();

		var packedBin = new Dimensions<int>
		{
			Length = result.Bin.Length, Width = result.Bin.Width, Height = result.Bin.Height,
		};

		// Flag anything that did not fully place. Bischoff instances are PartiallyPacked by design (fill ~98%),
		// so those leftovers are expected; a NotPacked / EarlyExit is worth a louder note. Still emitted either way.
		var unpackedCount = result.UnpackedItems.Sum(u => u.Quantity);
		if (unpackedCount > 0 ||
			result.Status is not (OperationResultStatus.FullyPacked or OperationResultStatus.PartiallyPacked))
		{
			Console.WriteLine(
				$"    ! {scenario.Name}: status={result.Status}, placed={placed.Count}, unpacked={unpackedCount}");
		}

		// Round-trip gate: the placed geometry must survive an encode → decode unchanged. We only need this to
		// prove the data is valid; the token itself is not stored — the kernel computes it when it benchmarks.
		var roundTripped = RoundTrips(packedBin, placed);
		if (!roundTripped)
		{
			Console.WriteLine($"    X {scenario.Name}: placed data did NOT round-trip!");
		}

		var sample = new PackedSample
		{
			Name = scenario.Name,
			WidthBits = DeriveWidthBits(packedBin, placed),
			Bin = CompactNotationFormatter.FormatDimensions(packedBin),
			Items = placed.Select(CompactNotationFormatter.FormatItem).ToArray(),
		};
		return (sample, roundTripped);
	}

	private static bool RoundTrips(Dimensions<int> bin, List<Item<int>> items)
	{
		var bytes = ViPaqSerializer.SerializeInt32(bin, items);
		var (decodedBin, decodedItems) = ViPaqSerializer.DeserializeInt32<Dimensions<int>, Item<int>>(bytes);

		if (decodedBin.Length != bin.Length || decodedBin.Width != bin.Width || decodedBin.Height != bin.Height)
		{
			return false;
		}

		if (decodedItems.Count != items.Count)
		{
			return false;
		}

		for (var i = 0; i < items.Count; i++)
		{
			var expected = items[i];
			var actual = decodedItems[i];
			if (actual.Length != expected.Length || actual.Width != expected.Width || actual.Height != expected.Height ||
				actual.X != expected.X || actual.Y != expected.Y || actual.Z != expected.Z)
			{
				return false;
			}
		}

		return true;
	}

	// 8 if every bin dimension, item dimension and coordinate fits in a byte; 16 otherwise. This is the width
	// family the sample belongs to; ViPaq chooses the actual per-section width from the values at encode time.
	private static int DeriveWidthBits(Dimensions<int> bin, List<Item<int>> items)
	{
		var max = Math.Max(bin.Length, Math.Max(bin.Width, bin.Height));
		foreach (var item in items)
		{
			max = Math.Max(max, item.Length);
			max = Math.Max(max, item.Width);
			max = Math.Max(max, item.Height);
			max = Math.Max(max, item.X);
			max = Math.Max(max, item.Y);
			max = Math.Max(max, item.Z);
		}

		return max <= 255 ? 8 : 16;
	}

	// A source family: its input directory (segments under the repo root), the destination subfolder for its
	// placed results, and the problem files it contains.
	private sealed record SourceFamily(string[] InputDir, string DestinationFolder, string[] Files);
}
