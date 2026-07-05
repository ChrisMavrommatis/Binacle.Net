using Binacle.ViPaq.UnitTests.Providers;
using Version = Binacle.ViPaq.Version;

namespace Binacle.ViPaq.UnitTests;

// CreateEncodingInfo picks a storage size for the bin, the item dimensions, and the item
// coordinates, each from its own values, takes the biggest item in the list, and starts the
// version as Uncompressed. Deciding "this value -> that size" is BitSizeHelper's job (tested
// there), so here we only check the wiring, the "biggest wins" loop, and the empty-list default.
// One wide type (ulong) is used because it can hold a value in every size bucket.
[Trait("Result Tests", "Ensures results are as expected")]
public class CreateEncodingInfoTests
{
	// A value whose required storage is exactly this size: the top of each bucket, except 64-bit which
	// is the bottom (smallest value that needs 64 bits). This forces BitSizeHelper to pick this exact
	// size. Different job from BitSizeValues.DistinctValue, which picks a comfortable interior value.
	private static ulong BoundaryValueFor(BitSize size) => size switch
	{
		BitSize.Eight => byte.MaxValue,                 // 255
		BitSize.Sixteen => ushort.MaxValue,             // 65535
		BitSize.ThirtyTwo => uint.MaxValue,             // 4294967295
		BitSize.SixtyFour => (ulong)uint.MaxValue + 1,  // 4294967296
		_ => throw new ArgumentOutOfRangeException(nameof(size))
	};

	private static Binacle.Geometry.Dimensions<ulong> BinOf(BitSize size)
	{
		var sizeValue = BoundaryValueFor(size);
		return new Binacle.Geometry.Dimensions<ulong> { Length = sizeValue, Width = sizeValue, Height = sizeValue };
	}

	private static Binacle.Geometry.Item<ulong> ItemOf(BitSize dimensionsSize, BitSize coordinatesSize)
	{
		var dimensionsSizeValue = BoundaryValueFor(dimensionsSize);
		var coordinatesSizeValue = BoundaryValueFor(coordinatesSize);

		return new Binacle.Geometry.Item<ulong>()
		{
			Length = dimensionsSizeValue, Width = dimensionsSizeValue, Height = dimensionsSizeValue,
			X = coordinatesSizeValue, Y = coordinatesSizeValue, Z = coordinatesSizeValue
		};
	}

	private static EncodingInfo CreateFor(Binacle.Geometry.Dimensions<ulong> bin, params Binacle.Geometry.Item<ulong>[] items) =>
		EncodingInfoHelper.CreateEncodingInfo<Binacle.Geometry.Dimensions<ulong>, Binacle.Geometry.Item<ulong>, ulong>(bin, items);

	// Wiring matrix: each of the three sizes comes from its own input, version starts Uncompressed.
	[Theory]
	[ClassData(typeof(CreateEncodingInfoSizeProvider))]
	public void CreateEncodingInfo_Sets_Each_Size_From_Its_Own_Input(
		BitSize binSize,
		BitSize itemDimensionsSize,
		BitSize itemCoordinatesSize)
	{
		var encodingInfo = CreateFor(BinOf(binSize), ItemOf(itemDimensionsSize, itemCoordinatesSize));

		encodingInfo.BinDimensionsBitSize.ShouldBe(binSize);
		encodingInfo.ItemDimensionsBitSize.ShouldBe(itemDimensionsSize);
		encodingInfo.ItemCoordinatesBitSize.ShouldBe(itemCoordinatesSize);
		encodingInfo.Version.ShouldBe(Version.Uncompressed);
	}

	[Fact]
	public void CreateEncodingInfo_Uses_The_Largest_Item_Dimensions()
	{
		// Items out of order; the biggest dimensions are in the middle of the list.
		var small = ItemOf(BitSize.Eight, BitSize.Eight);
		var big = ItemOf(BitSize.ThirtyTwo, BitSize.Eight);
		var medium = ItemOf(BitSize.Sixteen, BitSize.Eight);

		var encodingInfo = CreateFor(BinOf(BitSize.Eight), small, big, medium);

		encodingInfo.ItemDimensionsBitSize.ShouldBe(BitSize.ThirtyTwo);
	}

	[Fact]
	public void CreateEncodingInfo_Uses_The_Largest_Item_Coordinates()
	{
		var small = ItemOf(BitSize.Eight, BitSize.Eight);
		var big = ItemOf(BitSize.Eight, BitSize.SixtyFour);

		var encodingInfo = CreateFor(BinOf(BitSize.Eight), small, big);

		encodingInfo.ItemCoordinatesBitSize.ShouldBe(BitSize.SixtyFour);
	}

	[Fact]
	public void CreateEncodingInfo_Defaults_Item_Sizes_To_Eight_When_No_Items()
	{
		var encodingInfo = CreateFor(BinOf(BitSize.SixtyFour));

		encodingInfo.BinDimensionsBitSize.ShouldBe(BitSize.SixtyFour);
		encodingInfo.ItemDimensionsBitSize.ShouldBe(BitSize.Eight);
		encodingInfo.ItemCoordinatesBitSize.ShouldBe(BitSize.Eight);
		encodingInfo.Version.ShouldBe(Version.Uncompressed);
	}
}
