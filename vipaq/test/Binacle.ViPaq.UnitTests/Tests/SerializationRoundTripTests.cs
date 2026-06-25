using Binacle.ViPaq.UnitTests.Models;
using Binacle.ViPaq.UnitTests.Providers;
using static Binacle.ViPaq.UnitTests.SerializationTestingFixture;

namespace Binacle.ViPaq.UnitTests;

// A bin and its items come back unchanged through Serialize -> Deserialize, across types, widths,
// and the special cases (zero coordinates, no items, a compressed body).
[Trait("Result Tests", "Ensures results are as expected")]
public class SerializationRoundTripTests
{
	// type from the row -> the round trip done with that type (a type cannot live in a data row).
	private static readonly Dictionary<Type, Action<BitSize, BitSize, BitSize>> RoundTripByType = new()
	{
		[typeof(ushort)] = (bin, dim, coord) => AssertRoundTrips(BuildBin<ushort>(bin), [BuildItem<ushort>(dim, coord)]),
		[typeof(int)] = (bin, dim, coord) => AssertRoundTrips(BuildBin<int>(bin), [BuildItem<int>(dim, coord)]),
		[typeof(ulong)] = (bin, dim, coord) => AssertRoundTrips(BuildBin<ulong>(bin), [BuildItem<ulong>(dim, coord)]),
	};

	[Theory]
	[ClassData(typeof(SerializationRoundTripProvider))]
	public void RoundTrips_Across_Type_And_Widths(
		Type numericType,
		BitSize binSize,
		BitSize itemDimensionsSize,
		BitSize itemCoordinatesSize)
	{
		RoundTripByType[numericType](binSize, itemDimensionsSize, itemCoordinatesSize);
	}

	[Fact]
	public void RoundTrips_When_Coordinates_Are_Zero()
	{
		var bin = BuildBin<int>(BitSize.Eight);
		var item = new Item<int> { Length = 1, Width = 2, Height = 3, X = 0, Y = 0, Z = 0 };

		AssertRoundTrips(bin, [item]);
	}

	[Fact]
	public void RoundTrips_When_There_Are_No_Items()
	{
		AssertRoundTrips(BuildBin<int>(BitSize.Eight), new List<Item<int>>());
	}

	[Fact]
	public void RoundTrips_When_Body_Is_Compressed()
	{
		// 60 small items push the body well over the 255-byte compression threshold.
		var items = Enumerable.Range(0, 60)
			.Select(_ => BuildItem<int>(BitSize.Eight, BitSize.Eight))
			.ToList();

		AssertRoundTrips(BuildBin<int>(BitSize.Eight), items);
	}
}
