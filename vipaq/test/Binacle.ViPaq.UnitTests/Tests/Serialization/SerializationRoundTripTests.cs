using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// A bin and its items come back unchanged through encode -> decode, across types, widths and the special cases
// (zero coordinates, no items, a large body). Runs through ProtocolEncoder under the library's default header,
// so it covers the shape a real caller hits.
[Trait("Result Tests", "Ensures results are as expected")]
public class SerializationRoundTripTests
{
	// One method per numeric type rather than a type column: a type cannot live in a data row, and looking it
	// up means calling the round trip through a delegate, which hides the assertion. Width is internal, so a
	// public [Theory] cannot name it (CS0051): the boxed widths ride the row as object.
	[Theory]
	[ClassData(typeof(WidthCombinationsProvider))]
	public void RoundTrips_Across_Widths_As_UShort(
		object binWidthValue,
		object itemDimensionsWidthValue,
		object itemCoordinatesWidthValue)
	{
		var bin = BinContents.BuildBin<ushort>((Width)binWidthValue);
		var item = BinContents.BuildItem<ushort>((Width)itemDimensionsWidthValue, (Width)itemCoordinatesWidthValue);
		var expected = new BinContents<ushort>(bin, [item]);

		var actual = ProtocolTestingFixture.RoundTrip(expected);

		BinContents.AssertSame(expected, actual);
	}

	[Theory]
	[ClassData(typeof(WidthCombinationsProvider))]
	public void RoundTrips_Across_Widths_As_Int(
		object binWidthValue,
		object itemDimensionsWidthValue,
		object itemCoordinatesWidthValue)
	{
		var bin = BinContents.BuildBin<int>((Width)binWidthValue);
		var item = BinContents.BuildItem<int>((Width)itemDimensionsWidthValue, (Width)itemCoordinatesWidthValue);
		var expected = new BinContents<int>(bin, [item]);

		var actual = ProtocolTestingFixture.RoundTrip(expected);

		BinContents.AssertSame(expected, actual);
	}

	[Theory]
	[ClassData(typeof(WidthCombinationsProvider))]
	public void RoundTrips_Across_Widths_As_ULong(
		object binWidthValue,
		object itemDimensionsWidthValue,
		object itemCoordinatesWidthValue)
	{
		var bin = BinContents.BuildBin<ulong>((Width)binWidthValue);
		var item = BinContents.BuildItem<ulong>((Width)itemDimensionsWidthValue, (Width)itemCoordinatesWidthValue);
		var expected = new BinContents<ulong>(bin, [item]);

		var actual = ProtocolTestingFixture.RoundTrip(expected);

		BinContents.AssertSame(expected, actual);
	}

	[Fact]
	public void RoundTrips_When_Coordinates_Are_Zero()
	{
		var bin = BinContents.BuildBin<int>(Width.Eight);
		var items = new List<Binacle.Geometry.Item<int>>
		{
			new() { Length = 1, Width = 2, Height = 3, X = 0, Y = 0, Z = 0 },
		};
		var expected = new BinContents<int>(bin, items);

		var actual = ProtocolTestingFixture.RoundTrip(expected);

		BinContents.AssertSame(expected, actual);
	}

	[Fact]
	public void RoundTrips_When_There_Are_No_Items()
	{
		var bin = BinContents.BuildBin<int>(Width.Eight);
		var items = new List<Binacle.Geometry.Item<int>>();
		var expected = new BinContents<int>(bin, items);

		var actual = ProtocolTestingFixture.RoundTrip(expected);

		BinContents.AssertSame(expected, actual);
	}

	[Fact]
	public void RoundTrips_With_A_Large_Body()
	{
		// 60 small items, so this checks a large uncompressed body round-trips intact.
		var bin = BinContents.BuildBin<int>(Width.Eight);
		var items = Enumerable.Range(0, 60)
			.Select(_ => BinContents.BuildItem<int>(Width.Eight, Width.Eight))
			.ToList();
		var expected = new BinContents<int>(bin, items);

		var actual = ProtocolTestingFixture.RoundTrip(expected);

		BinContents.AssertSame(expected, actual);
	}

	[Fact]
	public void RoundTrips_Multiple_Distinct_Items()
	{
		// Every field of every item differs, so a serializer that swaps, drops or reorders items shows up as a
		// mismatch. The matrix above uses one item per row and the large-body case 60 identical ones, so
		// neither can catch an item-loop bug.
		var bin = BinContents.BuildBin<int>(Width.Eight);
		var items = new List<Binacle.Geometry.Item<int>>
		{
			new() { Length = 1, Width = 2, Height = 3, X = 10, Y = 11, Z = 12 },
			new() { Length = 4, Width = 5, Height = 6, X = 13, Y = 14, Z = 15 },
			new() { Length = 7, Width = 8, Height = 9, X = 16, Y = 17, Z = 18 },
		};
		var expected = new BinContents<int>(bin, items);

		var actual = ProtocolTestingFixture.RoundTrip(expected);

		BinContents.AssertSame(expected, actual);
	}

	[Fact]
	public void RoundTrips_When_Item_Count_Exceeds_255()
	{
		// The item count is a little-endian uint16. Under 256 its high byte is always 0, so a bug in that
		// second byte would stay hidden; 300 items force a non-zero high byte (300 = 0x012C).
		var bin = BinContents.BuildBin<int>(Width.Eight);
		var items = Enumerable.Range(0, 300)
			.Select(i => new Binacle.Geometry.Item<int>
			{
				Length = i + 1, Width = i + 1_000, Height = i + 2_000,
				X = i, Y = i + 10_000, Z = i + 20_000,
			})
			.ToList();
		var expected = new BinContents<int>(bin, items);

		var actual = ProtocolTestingFixture.RoundTrip(expected);

		BinContents.AssertSame(expected, actual);
	}
}
