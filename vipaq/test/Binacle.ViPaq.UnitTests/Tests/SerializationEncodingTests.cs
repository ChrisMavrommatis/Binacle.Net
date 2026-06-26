using Binacle.ViPaq.UnitTests.Models;
using Binacle.ViPaq.UnitTests.Providers;
using Version = Binacle.ViPaq.Version;

namespace Binacle.ViPaq.UnitTests;

// The exact bytes on the wire. This is the anchor for the format: header byte, little-endian
// item count, then bin dimensions, then each item's dimensions and coordinates. The TypeScript
// mirror must produce the same bytes for the same input, so this is the cross-language contract.
[Trait("Result Tests", "Ensures results are as expected")]
public class SerializationEncodingTests
{
	// Hand-derived golden vectors: known input -> the exact bytes it must produce. The provider
	// covers a single 8-bit item, a 16-bit bin, two items, and an item at the origin (see
	// ExactBytesProvider). Each row pins byte order, header packing, and field order at once.
	[Theory]
	[ClassData(typeof(ExactBytesProvider))]
	public void Serialize_Produces_Exact_Bytes(Bin<int> bin, Item<int>[] items, byte[] expected)
	{
		var data = ViPaqSerializer.SerializeInt32<Bin<int>, Item<int>>(bin, items);

		data.ShouldBe(expected);
	}

	// Each section's size in the header comes from its own input, and a small body stays Uncompressed.
	[Theory]
	[ClassData(typeof(CreateEncodingInfoSizeProvider))]
	public void Serialize_Writes_Correct_Sizes_In_Header(
		BitSize binSize,
		BitSize itemDimensionsSize,
		BitSize itemCoordinatesSize)
	{
		var bin = SerializationTestingFixture.BuildBin<ulong>(binSize);
		var item = SerializationTestingFixture.BuildItem<ulong>(itemDimensionsSize, itemCoordinatesSize);

		var data = ViPaqSerializer.Serialize<Bin<ulong>, Item<ulong>, ulong>(bin, [item]);
		var header = EncodingInfoHelper.FromByte(data[0]);

		header.BinDimensionsBitSize.ShouldBe(binSize);
		header.ItemDimensionsBitSize.ShouldBe(itemDimensionsSize);
		header.ItemCoordinatesBitSize.ShouldBe(itemCoordinatesSize);
		header.Version.ShouldBe(Version.Uncompressed);
	}

	// Version records whether the body was gzip-compressed, and that switch is only about body
	// length: > 255 bytes compresses, <= 255 stays raw (ViPaqSerializer.Serialize.cs,
	// `memoryStream.Length > byte.MaxValue`). Body = 2 (item count) + 3*binBytes + N*(3*dimBytes +
	// 3*coordBytes). Everything but the 2-byte count is a multiple of 3, so every reachable body
	// length is `2 mod 3` — no input can land exactly 255 or 256. The real neighbours of the
	// threshold are 254 (largest body that stays raw) and 257 (smallest that compresses); the last
	// two rows pin the switch to that gap.
	[Theory]
	[InlineData(BitSize.Eight,   1,  Version.Uncompressed)]   // tiny body
	[InlineData(BitSize.Eight,   60, Version.CompressedGzip)] // well over the threshold
	[InlineData(BitSize.Sixteen, 41, Version.Uncompressed)]   // 254 = 2 + 6 + 246, largest raw body
	[InlineData(BitSize.Eight,   42, Version.CompressedGzip)] // 257 = 2 + 3 + 252, just over
	public void Serialize_Sets_Version_From_Body_Size(BitSize binSize, int itemCount, Version expected)
	{
		var bin = SerializationTestingFixture.BuildBin<int>(binSize);
		var items = Enumerable.Range(0, itemCount)
			.Select(_ => SerializationTestingFixture.BuildItem<int>(BitSize.Eight, BitSize.Eight))
			.ToList();

		var data = ViPaqSerializer.SerializeInt32<Bin<int>, Item<int>>(bin, items);

		EncodingInfoHelper.FromByte(data[0]).Version.ShouldBe(expected);
	}
}
