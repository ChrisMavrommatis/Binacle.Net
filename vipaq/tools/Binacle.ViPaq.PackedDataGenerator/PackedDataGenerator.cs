using System.Text.Encodings.Web;
using System.Text.Json;
using Binacle.CompactNotation;
using Binacle.Lib;
using Binacle.TestReporting;
using LibModels = Binacle.Lib.Models;

namespace Binacle.ViPaq.PackedDataGenerator;

// Packs one source family with one algorithm and writes the placed results into a subfolder of
// vipaq/data/packed. The algorithm rides on the file name as a ".<algo>" suffix, so a second algorithm sits
// beside this one in the same folder. Packing reproduces the API's exact call path.
//
// One generator is one algorithm, fixed for the run. The output is pure geometry and carries no ViPaq bytes.
internal sealed class PackedDataGenerator
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
		// Keep coordinate commas and 'x' literal instead of \u escapes, so the committed files stay readable.
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
	};

	// Path segments under the repo root. Each family gets its own subfolder.
	private static readonly string[] OutputRoot = ["vipaq", "data", "packed"];

	private readonly AlgorithmFactory factory;

	public PackedDataGenerator(Algorithm algorithm)
	{
		this.Algorithm = algorithm;
		this.factory = new AlgorithmFactory();
	}

	// The caller writes the per-algorithm log lines, so it needs this name.
	public Algorithm Algorithm { get; }

	public async Task<GenerationResult> GenerateAsync(SourceFamily family, RepositoryRootLocator locator)
	{
		var suffix = this.Algorithm.ToString().ToLowerInvariant();
		var inputDir = locator.Find(family.InputDir);
		var destinationDir = locator.Find([.. OutputRoot, family.DestinationFolder]);
		Directory.CreateDirectory(destinationDir);

		var totalSamples = 0;
		var totalItems = 0;

		foreach (var file in family.Files)
		{
			var scenarios = JsonSerializer.Deserialize<SourceScenario[]>(
				await File.ReadAllTextAsync(Path.Combine(inputDir, file)), ReadOptions)
				?? throw new InvalidOperationException($"{file} deserialized to null.");

			var samples = new List<PackedSample>(scenarios.Length);
			var fileItems = 0;
			foreach (var scenario in scenarios)
			{
				var sample = this.PackScenario(scenario);
				samples.Add(sample);
				fileItems += sample.Items.Length;
			}

			var outputFile = $"{Path.GetFileNameWithoutExtension(file)}.{suffix}.json";
			await File.WriteAllTextAsync(
				Path.Combine(destinationDir, outputFile), JsonSerializer.Serialize(samples, WriteOptions) + "\n");

			totalSamples += samples.Count;
			totalItems += fileItems;
			Console.WriteLine($"  {family.DestinationFolder}/{outputFile}: {samples.Count} samples, {fileItems} placed items");
		}

		return new GenerationResult(totalSamples, totalItems);
	}

	private PackedSample PackScenario(SourceScenario scenario)
	{
		var binDims = CompactNotationParser.ParseDimensions<int>(scenario.Bin);
		var bin = new LibModels.Bin("bin", binDims.Length, binDims.Width, binDims.Height);

		var items = new List<LibModels.Item>();
		var index = 0;
		foreach (var itemString in scenario.Items)
		{
			// Item types with a quantity: the algorithm expands it.
			var itemType = CompactNotationParser.ParseDimensionsAndQuantity<int>(itemString);
			items.Add(
				new LibModels.Item(
					$"item-{index++}",
					itemType.Length,
					itemType.Width,
					itemType.Height,
					itemType.Quantity
				)
			);
		}

		var result = this.factory.Create(this.Algorithm, bin, items).Execute(new PackingOperationParameters());

		// Bischoff instances are PartiallyPacked by design (fill ~98%), so those leftovers are expected. A
		// NotPacked / EarlyExit is worth a louder note. Emitted either way.
		var unpackedCount = result.UnpackedItems.Sum(unpacked => unpacked.Quantity);
		if (unpackedCount > 0 ||
			result.Status is not (OperationResultStatus.FullyPacked or OperationResultStatus.PartiallyPacked))
		{
			Console.WriteLine(
				$"    ! {scenario.Name}: status={result.Status}, placed={result.PackedItems.Count}, unpacked={unpackedCount}");
		}

		var sample = new PackedSample
		{
			Name = scenario.Name,
			WidthBits = DeriveWidthBits(result.Bin, result.PackedItems),
			Bin = CompactNotationFormatter.FormatDimensions(result.Bin),
			Items = result.PackedItems.Select(CompactNotationFormatter.FormatItem).ToArray(),
		};
		return sample;
	}

	// The sample's width family, not what ViPaq stores: ViPaq picks the per-section width at encode time.
	private static int DeriveWidthBits(PackedBin bin, IReadOnlyList<PackedItem> items)
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

		return max <= Limits.EightBitsMax ? 8 : 16;
	}
}
