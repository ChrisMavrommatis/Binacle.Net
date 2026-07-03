namespace Binacle.CompactNotation.UnitTests;

// Everything is parsed as long — it holds the whole interoperable range [0, 2^53-1] exactly and is
// the natural pair for JS number. Parse is lenient about range; it just reads the integers.
public class ParseTests
{
	[Theory]
	[InlineData("10x20x30", 10, 20, 30)]
	[InlineData(" 10x20x30 ", 10, 20, 30)] // trims outer space
	[InlineData("-5x3x2", -5, 3, 2)] // '-' is free now, so negatives parse
	[InlineData("0x0x0", 0, 0, 0)]
	public void ParseDimensions_reads_three_values_split_on_x(string compact, long l, long w, long h)
	{
		var dimensions = CompactNotation.ParseDimensions<long>(compact);

		dimensions.Length.ShouldBe(l);
		dimensions.Width.ShouldBe(w);
		dimensions.Height.ShouldBe(h);
	}

	[Theory]
	[InlineData("10x20")] // too few
	[InlineData("10x20x30x40")] // too many
	[InlineData("(1,2,3)")] // wrong block
	public void ParseDimensions_rejects_a_non_dimensions_string(string compact)
	{
		Should.Throw<FormatException>(() => CompactNotation.ParseDimensions<long>(compact));
	}

	[Theory]
	[InlineData("(1,2,3)", 1, 2, 3)]
	[InlineData(" (1,2,3) ", 1, 2, 3)]
	[InlineData("(-1,-2,-3)", -1, -2, -3)]
	public void ParseCoordinates_reads_three_values_inside_parens(string compact, long x, long y, long z)
	{
		var coordinates = CompactNotation.ParseCoordinates<long>(compact);

		coordinates.X.ShouldBe(x);
		coordinates.Y.ShouldBe(y);
		coordinates.Z.ShouldBe(z);
	}

	[Theory]
	[InlineData("1,2,3")] // missing parens
	[InlineData("(1,2)")] // too few
	[InlineData("10x20x30")] // wrong block
	public void ParseCoordinates_rejects_a_non_coordinates_string(string compact)
	{
		Should.Throw<FormatException>(() => CompactNotation.ParseCoordinates<long>(compact));
	}

	[Theory]
	[InlineData("[5]", 5)]
	[InlineData(" [12] ", 12)]
	[InlineData("[1]", 1)]
	public void ParseQuantity_reads_the_int_inside_brackets(string compact, int expected)
	{
		CompactNotation.ParseQuantity(compact).ShouldBe(expected);
	}

	[Theory]
	[InlineData("5")] // missing brackets
	[InlineData("[abc]")] // not a number
	public void ParseQuantity_rejects_a_non_quantity_string(string compact)
	{
		Should.Throw<FormatException>(() => CompactNotation.ParseQuantity(compact));
	}

	[Fact]
	public void ParseItem_reads_dimensions_and_coordinates()
	{
		var item = CompactNotation.ParseItem<long>("10x20x30 (1,2,3)");

		item.Length.ShouldBe(10);
		item.Width.ShouldBe(20);
		item.Height.ShouldBe(30);
		item.X.ShouldBe(1);
		item.Y.ShouldBe(2);
		item.Z.ShouldBe(3);
	}

	[Fact]
	public void ParseItem_rejects_a_quantity_suffix()
	{
		Should.Throw<FormatException>(() => CompactNotation.ParseItem<long>("10x20x30 (1,2,3) [3]"));
	}

	[Fact]
	public void ParseItem_rejects_a_missing_coordinate_block()
	{
		Should.Throw<FormatException>(() => CompactNotation.ParseItem<long>("10x20x30"));
	}

	[Fact]
	public void ParseItems_without_a_quantity_returns_one_item()
	{
		var items = CompactNotation.ParseItems<long>("10x20x30 (1,2,3)");

		items.Count.ShouldBe(1);
	}

	[Fact]
	public void ParseItems_expands_the_quantity_into_that_many_copies()
	{
		var items = CompactNotation.ParseItems<long>("10x20x30 (1,2,3) [3]");

		items.Count.ShouldBe(3);
		items.ShouldAllBe(item => item.Length == 10 && item.X == 1);
	}

	[Fact]
	public void ParseItems_returns_distinct_instances()
	{
		var items = CompactNotation.ParseItems<long>("1x1x1 (0,0,0) [2]");

		items[0].ShouldNotBeSameAs(items[1]);
	}

	[Fact]
	public void ParseItems_flattens_many_strings()
	{
		var items = CompactNotation.ParseItems<long>(new[] { "1x1x1 (0,0,0) [2]", "2x2x2 (1,1,1)" });

		items.Count.ShouldBe(3);
	}
}
