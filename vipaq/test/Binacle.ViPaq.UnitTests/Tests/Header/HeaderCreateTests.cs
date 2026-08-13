using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Header.Create picks a width for the bin, the item dimensions and the item coordinates, each from its own
// values, taking the biggest item in the list. "This value -> that width" is WidthHelper's job and is tested
// there, so this covers the wiring, the "biggest wins" loop and the empty-list default. ulong is used because
// it holds a value in every width bucket.
[Trait("Result Tests", "Ensures results are as expected")]
public class HeaderCreateTests
{
	// The top of each bucket, which forces WidthHelper to pick that exact width. Different job from
	// WidthValues.DistinctValue, which picks an interior value.
	private static ulong BoundaryValueFor(Width width) => width switch
	{
		Width.Eight => byte.MaxValue,     // 255
		Width.Sixteen => ushort.MaxValue, // 65535
		_ => throw new ArgumentOutOfRangeException(nameof(width))
	};

	private static Binacle.Geometry.Dimensions<ulong> BinOf(Width width)
	{
		var value = BoundaryValueFor(width);
		return new Binacle.Geometry.Dimensions<ulong> { Length = value, Width = value, Height = value };
	}

	private static Binacle.Geometry.Item<ulong> ItemOf(Width dimensionsWidth, Width coordinatesWidth)
	{
		var dimensionsValue = BoundaryValueFor(dimensionsWidth);
		var coordinatesValue = BoundaryValueFor(coordinatesWidth);

		return new Binacle.Geometry.Item<ulong>()
		{
			Length = dimensionsValue, Width = dimensionsValue, Height = dimensionsValue,
			X = coordinatesValue, Y = coordinatesValue, Z = coordinatesValue
		};
	}

	private static Header CreateFor(Binacle.Geometry.Dimensions<ulong> bin, params Binacle.Geometry.Item<ulong>[] items) =>
		Header.Create<Binacle.Geometry.Dimensions<ulong>, Binacle.Geometry.Item<ulong>, ulong>(bin, items);

	// Each of the three widths comes from its own input; the form is always raw + row-major. Width is internal,
	// so a public [Theory] cannot name it (CS0051): the boxed widths ride the row as object.
	[Theory]
	[ClassData(typeof(HeaderWidthComboProvider))]
	public void Create_Sets_Each_Width_From_Its_Own_Input(
		object binWidthValue,
		object itemDimensionsWidthValue,
		object itemCoordinatesWidthValue)
	{
		var binWidth = (Width)binWidthValue;
		var itemDimensionsWidth = (Width)itemDimensionsWidthValue;
		var itemCoordinatesWidth = (Width)itemCoordinatesWidthValue;

		var header = CreateFor(BinOf(binWidth), ItemOf(itemDimensionsWidth, itemCoordinatesWidth));

		header.BinDimensionsWidth.ShouldBe(binWidth);
		header.ItemDimensionsWidth.ShouldBe(itemDimensionsWidth);
		header.ItemCoordinatesWidth.ShouldBe(itemCoordinatesWidth);
		header.Compressed.ShouldBeFalse();
		header.Layout.ShouldBe(Layout.RowMajor);
	}

	[Fact]
	public void Create_Uses_The_Largest_Item_Dimensions()
	{
		// Items out of order; the biggest dimensions are in the middle of the list.
		var small = ItemOf(Width.Eight, Width.Eight);
		var big = ItemOf(Width.Sixteen, Width.Eight);
		var alsoSmall = ItemOf(Width.Eight, Width.Eight);

		var header = CreateFor(BinOf(Width.Eight), small, big, alsoSmall);

		header.ItemDimensionsWidth.ShouldBe(Width.Sixteen);
	}

	[Fact]
	public void Create_Uses_The_Largest_Item_Coordinates()
	{
		var small = ItemOf(Width.Eight, Width.Eight);
		var big = ItemOf(Width.Eight, Width.Sixteen);

		var header = CreateFor(BinOf(Width.Eight), small, big);

		header.ItemCoordinatesWidth.ShouldBe(Width.Sixteen);
	}

	[Fact]
	public void Create_Defaults_Item_Widths_To_Eight_When_No_Items()
	{
		var header = CreateFor(BinOf(Width.Sixteen));

		header.BinDimensionsWidth.ShouldBe(Width.Sixteen);
		header.ItemDimensionsWidth.ShouldBe(Width.Eight);
		header.ItemCoordinatesWidth.ShouldBe(Width.Eight);
		header.Compressed.ShouldBeFalse();
	}
}
