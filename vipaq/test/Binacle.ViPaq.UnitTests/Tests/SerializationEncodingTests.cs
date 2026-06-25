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
	[Fact]
	public void Serialize_Produces_Exact_Bytes_For_Known_Input()
	{
		var bin = new Bin<int> { Length = 10, Width = 20, Height = 30 };
		var item = new Item<int> { Length = 1, Width = 2, Height = 3, X = 4, Y = 5, Z = 6 };

		var data = ViPaqSerializer.SerializeInt32<Bin<int>, Item<int>>(bin, [item]);

		byte[] expected =
		[
			0b00_00_00_00,    // header: Uncompressed, bin/item-dim/item-coord all 8-bit
			0x01, 0x00,       // item count = 1, little-endian uint16
			0x0A, 0x14, 0x1E, // bin: length 10, width 20, height 30
			0x01, 0x02, 0x03, // item dimensions: length 1, width 2, height 3
			0x04, 0x05, 0x06, // item coordinates: x 4, y 5, z 6
		];

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

	[Fact]
	public void Serialize_Marks_Version_Uncompressed_When_Body_Is_Small()
	{
		var bin = SerializationTestingFixture.BuildBin<int>(BitSize.Eight);
		var item = SerializationTestingFixture.BuildItem<int>(BitSize.Eight, BitSize.Eight);

		var data = ViPaqSerializer.SerializeInt32<Bin<int>, Item<int>>(bin, [item]);

		EncodingInfoHelper.FromByte(data[0]).Version.ShouldBe(Version.Uncompressed);
	}

	[Fact]
	public void Serialize_Marks_Version_CompressedGzip_When_Body_Is_Large()
	{
		var bin = SerializationTestingFixture.BuildBin<int>(BitSize.Eight);
		// 60 small items push the body over the 255-byte compression threshold.
		var items = Enumerable.Range(0, 60)
			.Select(_ => SerializationTestingFixture.BuildItem<int>(BitSize.Eight, BitSize.Eight))
			.ToList();

		var data = ViPaqSerializer.SerializeInt32<Bin<int>, Item<int>>(bin, items);

		EncodingInfoHelper.FromByte(data[0]).Version.ShouldBe(Version.CompressedGzip);
	}
}
