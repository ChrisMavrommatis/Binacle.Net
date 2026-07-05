using System.Numerics;
using Binacle.ViPaq.Helpers;

namespace Binacle.ViPaq.UnitTests;

// Each numeric width caps at its own top bucket. For 8/16/32-bit types, using the type's MaxValue does
// double duty: it proves saturation caps correctly, and for the SIGNED types it is the Bug B regression
// guard — under CreateChecked the unsigned yardstick constant (e.g. ushort.MaxValue) overflows the signed
// sibling (short), so these would throw instead of returning the expected BitSize. The 64-bit bucket caps
// at MaxInteger (2^53 - 1), the protocol ceiling — below the 64-bit types' own max (PROTOCOL.md §5).
[Trait("Result Tests", "Ensures results are as expected")]
public class BitSizeHelperSaturationTests
{
	private static void AssertDimensions<T>(T value, BitSize expected)
		where T : struct, IBinaryInteger<T>
	{
		var dimensions = GeometryFactory.Dimensions(value, value, value);
		BitSizeHelper.GetDimensionsBitSize<Dimensions<T>, T>(dimensions)
			.ShouldBe(expected);
	}
	// Eight
	[Fact]
	public void Dimensions_Byte_Caps_At_Eight()
		=> AssertDimensions<byte>(byte.MaxValue, BitSize.Eight);

	[Fact]
	public void Dimensions_SByte_Caps_At_Eight()
		=> AssertDimensions<sbyte>(sbyte.MaxValue, BitSize.Eight);

	// Sixteen
	[Fact]
	public void Dimensions_UShort_Caps_At_Sixteen()
		=> AssertDimensions<ushort>(ushort.MaxValue, BitSize.Sixteen);

	[Fact]
	public void Dimensions_Short_Caps_At_Sixteen()
		=> AssertDimensions<short>(short.MaxValue, BitSize.Sixteen);

	// ThirtyTwo
	[Fact]
	public void Dimensions_UInt_Caps_At_ThirtyTwo()
		=> AssertDimensions<uint>(uint.MaxValue, BitSize.ThirtyTwo);

	[Fact]
	public void Dimensions_Int_Caps_At_ThirtyTwo()
		=> AssertDimensions<int>(int.MaxValue, BitSize.ThirtyTwo);

	// SixtyFour — caps at MaxInteger (2^53 - 1), not the type max: ulong/long hold more, but the protocol
	// rejects anything above MaxInteger (PROTOCOL.md §5), so that is the top of the bucket.
	[Fact]
	public void Dimensions_ULong_Caps_At_SixtyFour()
		=> AssertDimensions<ulong>(ViPaqLimits.MaxInteger, BitSize.SixtyFour);

	[Fact]
	public void Dimensions_Long_Caps_At_SixtyFour()
		=> AssertDimensions<long>((long)ViPaqLimits.MaxInteger, BitSize.SixtyFour);


	private static void AssertCoordinates<T>(T value, BitSize expected)
		where T : struct, IBinaryInteger<T>
	{
		var coordinates = GeometryFactory.Coordinates(value, value, value);
		BitSizeHelper.GetCoordinatesBitSize<Coordinates<T>, T>(coordinates)
			.ShouldBe(expected);
	}

	// Eight
	[Fact]
	public void Coordinates_Byte_Caps_At_Eight()
		=> AssertCoordinates<byte>(byte.MaxValue, BitSize.Eight);

	[Fact]
	public void Coordinates_SByte_Caps_At_Eight()
		=> AssertCoordinates<sbyte>(sbyte.MaxValue, BitSize.Eight);

	// Sixteen
	[Fact]
	public void Coordinates_UShort_Caps_At_Sixteen()
		=> AssertCoordinates<ushort>(ushort.MaxValue, BitSize.Sixteen);

	[Fact]
	public void Coordinates_Short_Caps_At_Sixteen()
		=> AssertCoordinates<short>(short.MaxValue, BitSize.Sixteen);

	// ThirtyTwo
	[Fact]
	public void Coordinates_UInt_Caps_At_ThirtyTwo()
		=> AssertCoordinates<uint>(uint.MaxValue, BitSize.ThirtyTwo);

	[Fact]
	public void Coordinates_Int_Caps_At_ThirtyTwo()
		=> AssertCoordinates<int>(int.MaxValue, BitSize.ThirtyTwo);

	// SixtyFour — caps at MaxInteger (2^53 - 1), not the type max (see the dimensions note above).
	[Fact]
	public void Coordinates_ULong_Caps_At_SixtyFour()
		=> AssertCoordinates<ulong>(ViPaqLimits.MaxInteger, BitSize.SixtyFour);

	[Fact]
	public void Coordinates_Long_Caps_At_SixtyFour()
		=> AssertCoordinates<long>((long)ViPaqLimits.MaxInteger, BitSize.SixtyFour);
}
