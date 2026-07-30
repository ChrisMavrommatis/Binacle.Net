using Binacle.ViPaq.UnitTests.Providers;
using static Binacle.ViPaq.UnitTests.SerializationTestingFixture;

namespace Binacle.ViPaq.UnitTests;

// A bin and its items come back unchanged through encode -> decode, across types, widths, and the special
// cases (zero coordinates, no items, a large body). The round trip runs through ProtocolEncoder (via the
// fixture) under the library's default header — raw, row-major, narrowest — so it covers the same shape a
// real caller hits without going through ViPaqSerializer, which cannot be told a different header.
[Trait("Result Tests", "Ensures results are as expected")]
public class SerializationRoundTripTests
{
	// type from the row -> the round trip done with that type (a type cannot live in a data row).
	private static readonly Dictionary<Type, Action<Width, Width, Width>> RoundTripByType = new()
	{
		[typeof(ushort)] = (bin, dim, coord) => AssertRoundTrips(BuildBin<ushort>(bin), [BuildItem<ushort>(dim, coord)]),
		[typeof(int)] = (bin, dim, coord) => AssertRoundTrips(BuildBin<int>(bin), [BuildItem<int>(dim, coord)]),
		[typeof(ulong)] = (bin, dim, coord) => AssertRoundTrips(BuildBin<ulong>(bin), [BuildItem<ulong>(dim, coord)]),
	};

	// Width is internal, so a public [Theory] method cannot name it (CS0051); the boxed widths ride the theory
	// data as object and are cast back here.
	[Theory]
	[ClassData(typeof(SerializationRoundTripProvider))]
	public void RoundTrips_Across_Type_And_Widths(
		Type numericType,
		object binWidthValue,
		object itemDimensionsWidthValue,
		object itemCoordinatesWidthValue)
	{
		RoundTripByType[numericType]((Width)binWidthValue, (Width)itemDimensionsWidthValue, (Width)itemCoordinatesWidthValue);
	}

	[Fact]
	public void RoundTrips_When_Coordinates_Are_Zero()
	{
		var bin = BuildBin<int>(Width.Eight);
		var item = new Binacle.Geometry.Item<int> { Length = 1, Width = 2, Height = 3, X = 0, Y = 0, Z = 0 };

		AssertRoundTrips(bin, [item]);
	}

	[Fact]
	public void RoundTrips_When_There_Are_No_Items()
	{
		AssertRoundTrips(BuildBin<int>(Width.Eight), new List<Binacle.Geometry.Item<int>>());
	}

	[Fact]
	public void RoundTrips_With_A_Large_Body()
	{
		// 60 small items make a body far larger than a single item. The library never compresses (no codec
		// is chosen), so this just checks a large uncompressed body round-trips intact.
		var items = Enumerable.Range(0, 60)
			.Select(_ => BuildItem<int>(Width.Eight, Width.Eight))
			.ToList();

		AssertRoundTrips(BuildBin<int>(Width.Eight), items);
	}

	[Fact]
	public void RoundTrips_Multiple_Distinct_Items()
	{
		// Every field of every item is different, so a serializer that swaps, drops, or reorders
		// items shows up as a mismatch. The matrix above only ever uses one item per row, and the
		// large-body case uses 60 identical items — neither can catch an item-loop bug.
		var bin = BuildBin<int>(Width.Eight);
		var items = new List<Binacle.Geometry.Item<int>>
		{
			new() { Length = 1, Width = 2, Height = 3, X = 10, Y = 11, Z = 12 },
			new() { Length = 4, Width = 5, Height = 6, X = 13, Y = 14, Z = 15 },
			new() { Length = 7, Width = 8, Height = 9, X = 16, Y = 17, Z = 18 },
		};

		AssertRoundTrips(bin, items);
	}

	[Fact]
	public void RoundTrips_When_Item_Count_Exceeds_255()
	{
		// The item count is a little-endian uint16. Under 256 its high byte is always 0, so a bug
		// in that second byte would stay hidden. 300 items force a non-zero high byte (300 = 0x012C).
		// Each item carries its index in every field, so order is checked at scale too.
		var bin = BuildBin<int>(Width.Eight);
		var items = Enumerable.Range(0, 300)
			.Select(i => new Binacle.Geometry.Item<int>
			{
				Length = i + 1, Width = i + 1_000, Height = i + 2_000,
				X = i, Y = i + 10_000, Z = i + 20_000,
			})
			.ToList();

		AssertRoundTrips(bin, items);
	}
}
