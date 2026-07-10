using Binacle.TestReporting;

namespace Binacle.ViPaq.VectorGenerators;

// Emits every valid header combo (2 compressed x 2 layouts x 2 bin widths x 2 item-dim widths x 2 item-coord
// widths = 32) to header-bytes.json — the golden both suites read for header pack/unpack. Version is always
// Version1: reserved versions never reach the wire, so they pack to no bytes to pin. Compressed is the outer
// loop and item-coordinates the inner, so the file reads left to right in the same order as the notation. Each
// row names the header in HeaderNotation text form and carries the two bytes it packs to (Header.ToBytes),
// written as grouped binary so the bit layout stays human-checkable. Each row is a concrete HeaderByteVector,
// so the schema (field names) lives in that class; CompactJson writes one row per line so the file stays
// greppable.
public sealed class HeaderBytesGenerator : IVectorGenerator
{
	public void Generate()
	{
		var outputPath = RepositoryRoot.Bind().Find("vipaq", "test-vectors", "header-bytes.json");

		var compressedFlags = new[] { false, true };
		var layouts = new[] { Layout.RowMajor, Layout.Columnar };
		var widths = new[] { Width.Eight, Width.Sixteen };

		Span<byte> bytes = stackalloc byte[Header.ByteCount];
		var vectors = new List<HeaderByteVector>();
		foreach (var compressed in compressedFlags)
		foreach (var layout in layouts)
		foreach (var binWidth in widths)
		foreach (var itemDimWidth in widths)
		foreach (var itemCoordWidth in widths)
		{
			var header = new Header
			{
				Version = Version.Version1,
				Compressed = compressed,
				Layout = layout,
				BinDimensionsWidth = binWidth,
				ItemDimensionsWidth = itemDimWidth,
				ItemCoordinatesWidth = itemCoordWidth,
			};

			header.ToBytes(bytes);

			vectors.Add(new HeaderByteVector
			{
				Header = HeaderNotation.Format(header),
				// Byte 0 groups as [Version 2][Compressed 1][Layout 1][reserved 4]; byte 1 as
				// [Bin 2][ItemDim 2][ItemCoord 2][reserved 2] — the layout in Header.
				Bytes = [ToGroupedBinary(bytes[0], 2, 1, 1, 4), ToGroupedBinary(bytes[1], 2, 2, 2, 2)],
			});
		}

		File.WriteAllText(outputPath, CompactJson.SerializeArray(vectors));
		Console.WriteLine($"Wrote {vectors.Count} header rows to {outputPath}");
	}

	// A byte as "0b" + underscore-separated bit groups, e.g. ToGroupedBinary(0, 2, 1, 1, 4) -> "0b00_0_0_0000".
	// The group sizes must sum to 8.
	private static string ToGroupedBinary(byte value, params int[] groupSizes)
	{
		var bits = Convert.ToString(value, 2).PadLeft(8, '0');

		var groups = new string[groupSizes.Length];
		var offset = 0;
		for (var index = 0; index < groupSizes.Length; index++)
		{
			groups[index] = bits.Substring(offset, groupSizes[index]);
			offset += groupSizes[index];
		}

		return "0b" + string.Join('_', groups);
	}
}
