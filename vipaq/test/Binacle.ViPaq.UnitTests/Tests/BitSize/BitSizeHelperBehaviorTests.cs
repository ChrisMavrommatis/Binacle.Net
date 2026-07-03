using Binacle.ViPaq.Helpers;

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

	// Above MaxInteger (2^53 - 1) the value is outside ViPaq's range and is rejected. This is now reachable
	// with a 64-bit type (before the ceiling moved to MaxInteger, only a wider-than-64-bit T could trip it).
	public static IEnumerable<object[]> DimensionsOverMaxIntegerData =>
	[
		[ViPaqLimits.MaxInteger + 1, 1UL, 1UL, nameof(Dimensions<ulong>.Length)],
		[1UL, ViPaqLimits.MaxInteger + 1, 1UL, nameof(Dimensions<ulong>.Width)],
		[1UL, 1UL, ViPaqLimits.MaxInteger + 1, nameof(Dimensions<ulong>.Height)],
	];

	[Theory]
	[MemberData(nameof(DimensionsOverMaxIntegerData))]
	public void GetDimensionsBitSize_Throws_ArgumentOutOfRangeException_When_Value_Exceeds_MaxInteger(
		ulong length,
		ulong width,
		ulong height,
		string expectedThrownParamName)
	{
		var dimensions = Dimensions.Create(length, width, height);

		var exception = Should.Throw<ArgumentOutOfRangeException>(() =>
			BitSizeHelper.GetDimensionsBitSize<Dimensions<ulong>, ulong>(dimensions)
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

	public static IEnumerable<object[]> CoordinatesOverMaxIntegerData =>
	[
		[ViPaqLimits.MaxInteger + 1, 0UL, 0UL, nameof(Coordinates<ulong>.X)],
		[0UL, ViPaqLimits.MaxInteger + 1, 0UL, nameof(Coordinates<ulong>.Y)],
		[0UL, 0UL, ViPaqLimits.MaxInteger + 1, nameof(Coordinates<ulong>.Z)],
	];

	[Theory]
	[MemberData(nameof(CoordinatesOverMaxIntegerData))]
	public void GetCoordinatesBitSize_Throws_ArgumentOutOfRangeException_When_Value_Exceeds_MaxInteger(
		ulong x,
		ulong y,
		ulong z,
		string expectedThrownParamName)
	{
		var coordinates = Coordinates.Create(x, y, z);

		var exception = Should.Throw<ArgumentOutOfRangeException>(() =>
			BitSizeHelper.GetCoordinatesBitSize<Coordinates<ulong>, ulong>(coordinates)
		);
		exception.ParamName.ShouldBe(expectedThrownParamName);
	}
}
