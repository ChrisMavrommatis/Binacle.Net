using System.Text.Json;
using Binacle.CompactNotation;
using Binacle.ViPaq.TestsKernel.Files;
using Binacle.ViPaq.TestsKernel.Models;

namespace Binacle.ViPaq.TestsKernel;

// Reads the frozen placed-result files that Binacle.ViPaq.PackedDataGenerator emits under vipaq/data/packed
// (bischoff-suite/ and custom-problems/) and the .csproj embeds as "PackedData.<family>.<name>.<algo>.json", and
// turns their rows into Scenarios. Kept self-contained on purpose — the kernel does not reference the ViPaq
// UnitTests, so it has its own reader rather than borrowing VectorReader. Regenerate the data with the tool.
internal static class PackedDataReader
{
	private const string ResourcePrefix = "PackedData.";

	private static readonly JsonSerializerOptions Options = new()
	{
		// JSON keys and POCO properties are both PascalCase, so no case-insensitive matching is needed.
		PropertyNameCaseInsensitive = false,
		ReadCommentHandling = JsonCommentHandling.Skip,
	};

	// The scenarios from one family's embedded files (e.g. "bischoff-suite" or "custom-problems"). The file
	// provider returns the files in sorted order, so the flattened stream is stable.
	public static IEnumerable<Scenario> Read(string family)
	{
		var files = EmbeddedResourceFileProvider.ByPrefix(ResourcePrefix)
			.Where(file => file.Family == family);

		foreach (var file in files)
		{
			using var stream = file.OpenRead();

			var records = JsonSerializer.Deserialize<PackedRecord[]>(stream, Options)
				?? throw new InvalidOperationException($"Packed data '{file.Name}' deserialized to null.");

			foreach (var record in records)
			{
				yield return ToScenario(record);
			}
		}
	}

	private static Scenario ToScenario(PackedRecord record)
	{
		// Parse as int (the notation's natural width) then narrow to ushort with a checked cast. Placed values
		// fit comfortably — the largest Bischoff coordinate is ~587, far below ushort's 65535 — so the cast is
		// a guard, not a lossy conversion.
		var binInt = CompactNotationParser.ParseDimensions<int>(record.Bin);
		var bin = new Dimensions<ushort>
		{
			Length = checked((ushort)binInt.Length),
			Width = checked((ushort)binInt.Width),
			Height = checked((ushort)binInt.Height),
		};

		var items = new Item<ushort>[record.Items.Length];
		for (var index = 0; index < items.Length; index++)
		{
			var item = CompactNotationParser.ParseItem<int>(record.Items[index]);
			items[index] = new Item<ushort>
			{
				Length = checked((ushort)item.Length),
				Width = checked((ushort)item.Width),
				Height = checked((ushort)item.Height),
				X = checked((ushort)item.X),
				Y = checked((ushort)item.Y),
				Z = checked((ushort)item.Z),
			};
		}

		return new Scenario
		{
			Name = record.Name,
			WidthBits = record.WidthBits,
			Spread = "real",
			Bin = bin,
			Items = items,
		};
	}

	// One row as stored on disk: compact-notation strings plus the derived width family. No token is stored —
	// it is derivable from the geometry and its compressed bytes vary by runtime.
	private sealed class PackedRecord
	{
		public required string Name { get; init; }
		public required int WidthBits { get; init; }
		public required string Bin { get; init; }
		public required string[] Items { get; init; }
	}
}
