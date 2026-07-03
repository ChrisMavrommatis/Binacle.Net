using System.Text.Json;
using Binacle.ViPaq;
using EncodingInfo = Binacle.ViPaq.EncodingInfo;
using Version = Binacle.ViPaq.Version;

namespace Binacle.ViPaq.Generators;

// Emits all 256 header combos (4 versions x 4 bin widths x 4 item-dim widths x 4 item-coord widths) to
// encoding-info-bytes.json — the golden both suites read for header pack/unpack. Version is the outer loop
// and item-coordinates the inner, matching the file's order. The Byte column is the independent expected
// output (Version<<6 | Bin<<4 | ItemDim<<2 | ItemCoord), written as grouped binary so the golden stays
// human-checkable. Each row is a concrete EncodingInfoByteVector, so the schema (field names) lives in that
// class; the whole list is serialized in one go.
public sealed class EncodingInfoBytesGenerator : IVectorGenerator
{
	private static readonly JsonSerializerOptions WriteOptions = new()
	{
		WriteIndented = true,
		IndentCharacter = '\t',
		IndentSize = 1,
	};

	public void Generate()
	{
		var outputPath = Path.Combine(RepoLocator.FindTestVectorsDir(), "encoding-info-bytes.json");

		var versions = new[] { Version.Uncompressed, Version.CompressedGzip, Version.Reserved2, Version.Reserved3 };
		var widths = new[] { BitSize.Eight, BitSize.Sixteen, BitSize.ThirtyTwo, BitSize.SixtyFour };

		var vectors = new List<EncodingInfoByteVector>();
		foreach (var version in versions)
		foreach (var binWidth in widths)
		foreach (var itemDimWidth in widths)
		foreach (var itemCoordWidth in widths)
		{
			var encodingInfo = new EncodingInfo
			{
				Version = version,
				BinDimensionsBitSize = binWidth,
				ItemDimensionsBitSize = itemDimWidth,
				ItemCoordinatesBitSize = itemCoordWidth,
			};

			vectors.Add(new EncodingInfoByteVector
			{
				EncodingInfo = encodingInfo.ToLabel(),
				Byte = ToGroupedBinary(EncodingInfoHelper.ToByte(encodingInfo)),
			});
		}

		File.WriteAllText(outputPath, JsonSerializer.Serialize(vectors, WriteOptions));
		Console.WriteLine($"Wrote {vectors.Count} encoding-info rows to {outputPath}");
	}

	// A byte as "0b" + four underscore-separated 2-bit groups, e.g. 0 -> "0b00_00_00_00", 255 -> "0b11_11_11_11".
	private static string ToGroupedBinary(byte value)
	{
		var bits = Convert.ToString(value, 2).PadLeft(8, '0');
		return $"0b{bits[..2]}_{bits[2..4]}_{bits[4..6]}_{bits[6..8]}";
	}
}
