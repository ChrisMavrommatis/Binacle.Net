using System.Numerics;
using Binacle.ViPaq.Helpers;
using Binacle.ViPaq.UnitTests.Models;

namespace Binacle.ViPaq.UnitTests;

// Each numeric width caps at its own top bucket. Using a type's MaxValue does double duty:
// it proves saturation caps correctly, and for the SIGNED types it is the Bug B regression guard —
// under CreateChecked the unsigned yardstick constant (e.g. ushort.MaxValue) overflows the signed
// sibling (short), so these would throw instead of returning the expected BitSize.
[Trait("Result Tests", "Ensures results are as expected")]
public class BitSizeHelperSaturationTests
{
	private static void AssertDimensions<T>(T value, BitSize expected)
		where T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T>
	{
		var dimensions = Dimensions.Create(value, value, value);
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

	// SixtyFour
	[Fact] 
	public void Dimensions_ULong_Caps_At_SixtyFour() 
		=> AssertDimensions<ulong>(ulong.MaxValue, BitSize.SixtyFour);
	
	[Fact] 
	public void Dimensions_Long_Caps_At_SixtyFour() 
		=> AssertDimensions<long>(long.MaxValue, BitSize.SixtyFour);
	
	
	private static void AssertCoordinates<T>(T value, BitSize expected)
		where T : struct, IBinaryInteger<T>, INumber<T>, IComparable<T>
	{
		var coordinates = Coordinates.Create(value, value, value);
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

	// SixtyFour
	[Fact] 
	public void Coordinates_ULong_Caps_At_SixtyFour() 
		=> AssertCoordinates<ulong>(ulong.MaxValue, BitSize.SixtyFour);
	
	[Fact] 
	public void Coordinates_Long_Caps_At_SixtyFour() 
		=> AssertCoordinates<long>(long.MaxValue, BitSize.SixtyFour);
}
