using Binacle.ViPaq.Helpers;
using Binacle.ViPaq.UnitTests.Models;

namespace Binacle.ViPaq.UnitTests;

[Trait("Result Tests", "Ensures results are as expected")]
public class BitSizeHelperTests
{
	public static IEnumerable<object[]> DataCases =>
	[
		// Uniform
		[10UL, 20UL, 30UL, BitSize.Eight],
		[300UL, 400UL, 500UL, BitSize.Sixteen],
		[70_000UL, 80_000UL, 90_000UL, BitSize.ThirtyTwo],
		[5_000_000_000UL, 6_000_000_000UL, 7_000_000_000UL, BitSize.SixtyFour],

		// Largest Wins
		[300UL, 10UL, 10UL, BitSize.Sixteen],
		[10UL, 300UL, 10UL, BitSize.Sixteen],
		[10UL, 10UL, 300UL, BitSize.Sixteen],

		[70_000UL, 300UL, 300UL, BitSize.ThirtyTwo],
		[300UL, 70_000UL, 300UL, BitSize.ThirtyTwo],
		[300UL, 300UL, 70_000UL, BitSize.ThirtyTwo],

		[5_000_000_000UL, 70_000UL, 70_000UL, BitSize.SixtyFour],
		[70_000UL, 5_000_000_000UL, 70_000UL, BitSize.SixtyFour],
		[70_000UL, 70_000UL, 5_000_000_000UL, BitSize.SixtyFour],

		// Boundaries
		[255UL, 255UL, 255UL, BitSize.Eight],
		[256UL, 256UL, 256UL, BitSize.Sixteen],
		[65_535UL, 65_535UL, 65_535UL, BitSize.Sixteen],
		[65_536UL, 65_536UL, 65_536UL, BitSize.ThirtyTwo],
		[4_294_967_295UL, 4_294_967_295UL, 4_294_967_295UL, BitSize.ThirtyTwo],
		[4_294_967_296UL, 4_294_967_296UL, 4_294_967_296UL, BitSize.SixtyFour],
	];

	[Theory]
	[MemberData(nameof(DataCases))]
	public void GetDimensionsBitSize_Returns_Expected_BitSize(
		ulong length,
		ulong width,
		ulong height,
		BitSize expectedBitSize
		)
	{
		var dimensions = Dimensions.Create(length, width, height);
		BitSizeHelper.GetDimensionsBitSize<Dimensions<ulong>, ulong>(dimensions)
			.ShouldBe(expectedBitSize);
	}

	[Theory]
	[MemberData(nameof(DataCases))]
	public void GetCoordinatesBitSize_Returns_Expected_BitSize(
		ulong x,
		ulong y,
		ulong z,
		BitSize expectedBitSize
		)
	{
		var coordinates = Coordinates.Create(x, y, z);
		BitSizeHelper.GetCoordinatesBitSize<Coordinates<ulong>, ulong>(coordinates)
			.ShouldBe(expectedBitSize);
	}
}
