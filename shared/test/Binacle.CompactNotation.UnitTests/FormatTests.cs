namespace Binacle.CompactNotation.UnitTests;

public class FormatTests
{
	// A test object that carries all three blocks, to prove Format composes them in order.
	private sealed class Placed<T> : IWithReadOnlyDimensions<T>, IWithReadOnlyCoordinates<T>, IWithReadOnlyQuantity<T>
		where T : struct, System.Numerics.IBinaryInteger<T>
	{
		public required T Length { get; init; }
		public required T Width { get; init; }
		public required T Height { get; init; }
		public required T X { get; init; }
		public required T Y { get; init; }
		public required T Z { get; init; }
		public required T Quantity { get; init; }
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
	public void Format_a_dimensions_only_object_writes_one_block()
	{
		var dimensions = new Dimensions<long> { Length = 10, Width = 20, Height = 30 };

		CompactNotationFormatter.Format<long>(dimensions).ShouldBe("10x20x30");
	}

	[Fact]
	public void Format_an_item_writes_dimensions_then_coordinates()
	{
		var item = new Item<long> { Length = 10, Width = 20, Height = 30, X = 1, Y = 2, Z = 3 };

		CompactNotationFormatter.Format<long>(item).ShouldBe("10x20x30 (1,2,3)");
	}

	[Fact]
	public void Format_appends_every_block_the_object_carries()
	{
		var placed = new Placed<long>
		{
			Length = 10, Width = 20, Height = 30, X = 1, Y = 2, Z = 3, Quantity = 5,
		};

		CompactNotationFormatter.Format<long>(placed).ShouldBe("10x20x30 (1,2,3) [5]");
	}

	[Fact]
	public void Format_works_with_int_the_lib_number_type()
	{
		var dimensions = new Dimensions<int> { Length = 10, Width = 20, Height = 30 };

		CompactNotationFormatter.Format<int>(dimensions).ShouldBe("10x20x30");
	}

	[Fact]
	public void Format_rejects_an_object_with_no_block()
	{
		Should.Throw<ArgumentException>(() => CompactNotationFormatter.Format<long>(new object()));
	}
}
