using System.Numerics;
using Binacle.ViPaq.Helpers;
using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Every supported type checked at the value boundaries it can reach. A value cannot be put in a
// data row as type T, so a small lookup table turns the type from the row into the real call.
// Dimensions and coordinates share the same size thresholds, so one row grades both.
[Trait("Result Tests", "Ensures results are as expected")]
public class BitSizeHelperBoundaryTests
{
	// type from the row -> the dimension and coordinate size for a value of that type.
	private static readonly Dictionary<Type, Func<ulong, (BitSize Dimensions, BitSize Coordinates)>> BitSizesByType = new()
	{
		[typeof(byte)] = SizesFor<byte>,
		[typeof(sbyte)] = SizesFor<sbyte>,
		[typeof(short)] = SizesFor<short>,
		[typeof(ushort)] = SizesFor<ushort>,
		[typeof(int)] = SizesFor<int>,
		[typeof(uint)] = SizesFor<uint>,
		[typeof(long)] = SizesFor<long>,
		[typeof(ulong)] = SizesFor<ulong>,
	};

	private static (BitSize, BitSize) SizesFor<T>(ulong value)
		where T : struct, IBinaryInteger<T>
	{
		var typedValue = T.CreateChecked(value);
		var dimensions = BitSizeHelper.GetDimensionsBitSize<Dimensions<T>, T>(
			GeometryFactory.Dimensions(typedValue, typedValue, typedValue));
		var coordinates = BitSizeHelper.GetCoordinatesBitSize<Coordinates<T>, T>(
			GeometryFactory.Coordinates(typedValue, typedValue, typedValue));
		return (dimensions, coordinates);
	}

	[Theory]
	[ClassData(typeof(BitSizeBoundaryByTypeProvider))]
	public void GetBitSize_Returns_Expected_For_Type_At_Boundary(Type numericType, ulong value, BitSize expected)
	{
		var (dimensions, coordinates) = BitSizesByType[numericType](value);

		dimensions.ShouldBe(expected);
		coordinates.ShouldBe(expected);
	}

	// Coordinates allow zero (dimensions do not), so (0, 0, 0) is the lowest valid coordinate.
	[Fact]
	public void GetCoordinatesBitSize_Returns_Eight_When_All_Coordinates_Are_Zero()
	{
		var coordinates = GeometryFactory.Coordinates(0, 0, 0);

		BitSizeHelper.GetCoordinatesBitSize<Coordinates<int>, int>(coordinates).ShouldBe(BitSize.Eight);
	}
}
