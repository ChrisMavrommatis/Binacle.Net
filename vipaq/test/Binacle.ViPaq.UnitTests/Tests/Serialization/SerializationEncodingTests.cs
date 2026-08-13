using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// The exact bytes on the wire, and the cross-language contract: the TypeScript mirror must produce the same
// bytes for the same input. Both directions are pinned against the same golden vectors, which catches a bug
// symmetric in encode+decode that a round-trip test would miss.
//
// Encode goes through ProtocolEncoder, not ViPaqSerializer: the header is derived from the golden bytes and
// handed in, so the test pins the bytes a given header produces. All exact-bytes vectors are raw, because
// compressed bytes are not reproducible.
[Trait("Result Tests", "Ensures results are as expected")]
public class SerializationEncodingTests
{
	// Known input -> the exact bytes it must produce. Each case pins byte order, header packing and field
	// order at once.
	[Theory]
	[MemberData(nameof(ExactBytesProvider.Names), MemberType = typeof(ExactBytesProvider))]
	public void Encode_Produces_Exact_Bytes(string name)
	{
		var scenario = ExactBytesProvider.Get(name);
		var header = Header.FromBytes(scenario.Bytes[0], scenario.Bytes[1]);

		var data = ProtocolTestingFixture.Encode(header, scenario.Bin, scenario.Items);

		data.ShouldBe(scenario.Bytes);
	}

	// The inverse: the same known bytes decode back to the same bin and items. Pinning decode against literal
	// bytes catches a bug symmetric in serialize+deserialize.
	[Theory]
	[MemberData(nameof(ExactBytesProvider.Names), MemberType = typeof(ExactBytesProvider))]
	public void Decode_Produces_Exact_Object(string name)
	{
		var scenario = ExactBytesProvider.Get(name);
		var expected = new BinContents<long>(scenario.Bin, scenario.Items);

		var actual = ProtocolTestingFixture.Deserialize<long>(scenario.Bytes);

		BinContents.AssertSame(expected, actual);
	}

	// Stays on ViPaqSerializer on purpose: picking the narrowest width and leaving the blob uncompressed is its
	// own job (PROTOCOL.md §4/§6), and ProtocolEncoder cannot pin it. Width is internal, so the boxed widths
	// ride the row as object.
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

		var bin = BinContents.BuildBin<ulong>(binWidth);
		var item = BinContents.BuildItem<ulong>(itemDimensionsWidth, itemCoordinatesWidth);

		var data = ViPaqSerializer.Serialize<Binacle.Geometry.Dimensions<ulong>, Binacle.Geometry.Item<ulong>, ulong>(bin, [item]);
		var header = Header.FromBytes(data[0], data[1]);

		header.BinDimensionsWidth.ShouldBe(binWidth);
		header.ItemDimensionsWidth.ShouldBe(itemDimensionsWidth);
		header.ItemCoordinatesWidth.ShouldBe(itemCoordinatesWidth);
		header.Compressed.ShouldBeFalse();
	}
}
