using Binacle.ViPaq.Helpers;
using Binacle.ViPaq.UnitTests.Models;

namespace Binacle.ViPaq.UnitTests;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class BitSizeHelperBehaviorTests
{
	[Theory]
	// Zero is rejected (dimensions must be greater than zero), one field at a time
	[InlineData(0, 1, 1, nameof(Dimensions<int>.Length))]
	[InlineData(1, 0, 1, nameof(Dimensions<int>.Width))]
	[InlineData(1, 1, 0, nameof(Dimensions<int>.Height))]
	// Negative is rejected, one field at a time
	[InlineData(-1, 1, 1, nameof(Dimensions<int>.Length))]
	[InlineData(1, -1, 1, nameof(Dimensions<int>.Width))]
	[InlineData(1, 1, -1, nameof(Dimensions<int>.Height))]
	public void GetDimensionsBitSize_Throws_ArgumentOutOfRangeException_ForParamName(
		int length,
		int width,
		int height,
		string expectedThrownParamName)
	{
		var dimensions = Dimensions.Create(length, width, height);
		var exception =
			Should.Throw<ArgumentOutOfRangeException>(() =>
				BitSizeHelper.GetDimensionsBitSize<Dimensions<int>, int>(dimensions)
			);

		exception.ParamName.ShouldBe(expectedThrownParamName);
	}

	[Theory]
	[InlineData(-1, 0, 0, nameof(Coordinates<int>.X))]
	[InlineData(0, -1, 0, nameof(Coordinates<int>.Y))]
	[InlineData(0, 0, -1, nameof(Coordinates<int>.Z))]
	public void GetCoordinatesBitSize_Throws_ArgumentOutOfRangeException_ForParamName(
		int x,
		int y,
		int z,
		string expectedThrownParamName)
	{
		var coordinates = Coordinates.Create(x, y, z);
		var exception = Should.Throw<ArgumentOutOfRangeException>(() =>
			BitSizeHelper.GetCoordinatesBitSize<Coordinates<int>, int>(coordinates)
		);

		exception.ParamName.ShouldBe(expectedThrownParamName);
	}

	// A value that needs more than 64 bits is the format's ceiling — it cannot be encoded.
	// This is only reachable with a wider-than-64-bit T (here UInt128). For any type up to
	// 64 bits the SixtyFour branch always catches the value first, so the throw is dead code there.
	private static UInt128 ULongMax => ulong.MaxValue;

	public static IEnumerable<object[]> DimensionsOver64BitsData =>
	[
		[ULongMax + UInt128.One, ULongMax, ULongMax, nameof(Dimensions<UInt128>.Length)],
		[ULongMax, ULongMax + UInt128.One, ULongMax, nameof(Dimensions<UInt128>.Width)],
		[ULongMax, ULongMax, ULongMax + UInt128.One, nameof(Dimensions<UInt128>.Height)],
	];

	[Theory]
	[MemberData(nameof(DimensionsOver64BitsData))]
	public void GetDimensionsBitSize_Throws_ArgumentOutOfRangeException_When_Value_Exceeds_64_Bits(
		UInt128 length,
		UInt128 width,
		UInt128 height,
		string expectedThrownParamName
	)
	{
		var dimensions = Dimensions.Create(length, width, height);

		var exception = Should.Throw<ArgumentOutOfRangeException>(() =>
			BitSizeHelper.GetDimensionsBitSize<Dimensions<UInt128>, UInt128>(dimensions)
		);
		exception.ParamName.ShouldBe(expectedThrownParamName);
	}

	public static IEnumerable<object[]> CoordinatesOver64BitsData =>
	[
		[ULongMax + UInt128.One, ULongMax, ULongMax, nameof(Coordinates<UInt128>.X)],
		[ULongMax, ULongMax + UInt128.One, ULongMax, nameof(Coordinates<UInt128>.Y)],
		[ULongMax, ULongMax, ULongMax + UInt128.One, nameof(Coordinates<UInt128>.Z)],
	];

	[Theory]
	[MemberData(nameof(CoordinatesOver64BitsData))]
	public void GetCoordinatesBitSize_Throws_ArgumentOutOfRangeException_When_Value_Exceeds_64_Bits(
		UInt128 x,
		UInt128 y,
		UInt128 z,
		string expectedThrownParamName)
	{
		var coordinates = Coordinates.Create(x, y, z);

		var exception = Should.Throw<ArgumentOutOfRangeException>(() =>
			BitSizeHelper.GetCoordinatesBitSize<Coordinates<UInt128>, UInt128>(coordinates)
		);

		exception.ParamName.ShouldBe(expectedThrownParamName);
	}
}
