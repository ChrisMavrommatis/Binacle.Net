using Binacle.ViPaq.UnitTests.Providers;
using Version = Binacle.ViPaq.Version;

namespace Binacle.ViPaq.UnitTests;

// The exact bytes on the wire. This is the anchor for the format: header byte, little-endian
// item count, then bin dimensions, then each item's dimensions and coordinates. The TypeScript
// mirror must produce the same bytes for the same input, so this is the cross-language contract.
// Both directions are pinned against the same golden vectors: serialize -> bytes, and bytes ->
// object. Pinning both sides catches a bug that is symmetric in serialize+deserialize, which a
// round-trip test would miss.
[Trait("Result Tests", "Ensures results are as expected")]
public class SerializationEncodingTests
{
	// Golden vectors from the shared exact-bytes.json: known input -> the exact bytes it must produce.
	// The row carries the Name; the case (bin, items, bytes) is resolved by name. Each case pins byte
	// order, header packing, and field order at once. Values are read as `long` (the interoperable
	// range fits long exactly), so the 64-bit cases load too.
	[Theory]
	[MemberData(nameof(ExactBytesProvider.Names), MemberType = typeof(ExactBytesProvider))]
	public void Serialize_Produces_Exact_Bytes(string name)
	{
		var scenario = ExactBytesProvider.Get(name);

		var data = ViPaqSerializer.Serialize<Bin<long>, Item<long>, long>(scenario.Bin, scenario.Items);

		data.ShouldBe(scenario.Bytes);
	}

	// The inverse of the serialize golden: the same known bytes must decode back to the same bin and
	// items. This pins the decode path against literal bytes, so a bug that is symmetric in
	// serialize+deserialize (and would slip past the round-trip tests) is caught here.
	[Theory]
	[MemberData(nameof(ExactBytesProvider.Names), MemberType = typeof(ExactBytesProvider))]
	public void Deserialize_Produces_Exact_Object(string name)
	{
		var scenario = ExactBytesProvider.Get(name);

		SerializationTestingFixture.AssertDeserializesTo(scenario.Bytes, scenario.Bin, scenario.Items);
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
