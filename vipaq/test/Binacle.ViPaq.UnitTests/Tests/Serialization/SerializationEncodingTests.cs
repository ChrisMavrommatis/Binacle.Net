using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// The exact bytes on the wire. This is the anchor for the format: the two header bytes, little-endian
// item count, then bin dimensions, then each item's dimensions and coordinates. The TypeScript
// mirror must produce the same bytes for the same input, so this is the cross-language contract.
// Both directions are pinned against the same golden vectors: encode -> bytes, and bytes ->
// object. Pinning both sides catches a bug that is symmetric in encode+decode, which a
// round-trip test would miss.
//
// Encode goes through ProtocolEncoder, not ViPaqSerializer: the header is derived from the golden bytes and
// handed in, so the test pins the exact bytes a given header produces rather than whatever widths ViPaqSerializer
// happens to pick. All exact-bytes vectors are raw (compressed bytes are not reproducible, so they can never be
// pinned here), so the encoder gets the NoOp codec and the body stays byte-for-byte readable.
[Trait("Result Tests", "Ensures results are as expected")]
public class SerializationEncodingTests
{
	// Golden vectors from the shared exact-bytes.json: known input -> the exact bytes it must produce.
	// The row carries the Name; the case (bin, items, bytes) is resolved by name. Each case pins byte
	// order, header packing, and field order at once. Values are read as `long` (the interoperable
	// range fits long exactly).
	[Theory]
	[MemberData(nameof(ExactBytesProvider.Names), MemberType = typeof(ExactBytesProvider))]
	public void Encode_Produces_Exact_Bytes(string name)
	{
		var scenario = ExactBytesProvider.Get(name);
		var header = Header.FromBytes(scenario.Bytes[0], scenario.Bytes[1]);

		var data = SerializationTestingFixture.Encode(header, scenario.Bin, scenario.Items);

		data.ShouldBe(scenario.Bytes);
	}

	// The inverse of the serialize golden: the same known bytes must decode back to the same bin and
	// items. This pins the decode path against literal bytes, so a bug that is symmetric in
	// serialize+deserialize (and would slip past the round-trip tests) is caught here.
	[Theory]
	[MemberData(nameof(ExactBytesProvider.Names), MemberType = typeof(ExactBytesProvider))]
	public void Decode_Produces_Exact_Object(string name)
	{
		var scenario = ExactBytesProvider.Get(name);

		SerializationTestingFixture.AssertDeserializesTo(scenario.Bytes, scenario.Bin, scenario.Items);
	}

	// This one stays on ViPaqSerializer on purpose: picking the narrowest width that holds each section, and
	// leaving the blob uncompressed, is ViPaqSerializer's own job (the choosing layer, PROTOCOL.md §4/§6) — the
	// exact thing ProtocolEncoder does NOT do, since it is handed a header. So this is the right place to pin
	// that choice. Width is internal, so the boxed widths ride the theory data as object and are cast back here.
	[Theory]
	[ClassData(typeof(HeaderWidthComboProvider))]
	public void ViPaqSerializer_Chooses_Correct_Widths_In_Header(
		object binWidthValue,
		object itemDimensionsWidthValue,
		object itemCoordinatesWidthValue)
	{
		var binWidth = (Width)binWidthValue;
		var itemDimensionsWidth = (Width)itemDimensionsWidthValue;
		var itemCoordinatesWidth = (Width)itemCoordinatesWidthValue;

		var bin = SerializationTestingFixture.BuildBin<ulong>(binWidth);
		var item = SerializationTestingFixture.BuildItem<ulong>(itemDimensionsWidth, itemCoordinatesWidth);

		var data = ViPaqSerializer.Serialize<Binacle.Geometry.Dimensions<ulong>, Binacle.Geometry.Item<ulong>, ulong>(bin, [item]);
		var header = Header.FromBytes(data[0], data[1]);

		header.BinDimensionsWidth.ShouldBe(binWidth);
		header.ItemDimensionsWidth.ShouldBe(itemDimensionsWidth);
		header.ItemCoordinatesWidth.ShouldBe(itemCoordinatesWidth);
		header.Compressed.ShouldBeFalse();
	}
}
