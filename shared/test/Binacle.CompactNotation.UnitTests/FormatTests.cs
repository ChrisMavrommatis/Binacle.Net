namespace Binacle.CompactNotation.UnitTests;

public class FormatTests
{
	// Carries all three blocks, so one fixture serves the composite tests.
	private sealed class Placed : IWithReadOnlyDimensions<int>, IWithReadOnlyCoordinates<int>, IWithReadOnlyQuantity<int>
	{
		public required int Length { get; init; }
		public required int Width { get; init; }
		public required int Height { get; init; }
		public required int X { get; init; }
		public required int Y { get; init; }
		public required int Z { get; init; }
		public required int Quantity { get; init; }
	}

	[Fact]
	public void FormatDimensions_writes_LxWxH()
	{
		var dimensions = new Dimensions<long> { Length = 10, Width = 20, Height = 30 };

		CompactNotationFormatter.FormatDimensions(dimensions).ShouldBe("10x20x30");
	}

	[Fact]
	public void FormatCoordinates_writes_parens()
	{
		var coordinates = new Coordinates<long> { X = 1, Y = 2, Z = 3 };

		CompactNotationFormatter.FormatCoordinates(coordinates).ShouldBe("(1,2,3)");
	}

	[Fact]
	public void FormatQuantity_writes_brackets()
	{
		var placed = new Placed { Length = 10, Width = 20, Height = 30, X = 1, Y = 2, Z = 3, Quantity = 5 };

		CompactNotationFormatter.FormatQuantity(placed).ShouldBe("[5]");
	}

	[Fact]
	public void FormatItem_writes_dimensions_then_coordinates()
	{
		var placed = new Placed { Length = 10, Width = 20, Height = 30, X = 1, Y = 2, Z = 3, Quantity = 5 };

		CompactNotationFormatter.FormatItem(placed).ShouldBe("10x20x30 (1,2,3)");
	}

	[Fact]
	public void FormatDimensionsAndQuantity_writes_dimensions_then_quantity()
	{
		var placed = new Placed { Length = 10, Width = 20, Height = 30, X = 1, Y = 2, Z = 3, Quantity = 5 };

		CompactNotationFormatter.FormatDimensionsAndQuantity(placed).ShouldBe("10x20x30 [5]");
	}
}
