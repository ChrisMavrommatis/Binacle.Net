using System.Numerics;
using Binacle.ViPaq.Helpers;
using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Every supported type at the value boundaries it can reach. A value cannot go in a data row as type T, so a
// lookup table turns the type from the row into the real call. Dimensions and coordinates share the same width
// thresholds, so one row grades both.
[Trait("Result Tests", "Ensures results are as expected")]
public class WidthBoundaryByTypeTests
{
	// type from the row -> the dimension and coordinate width for a value of that type.
	private static readonly Dictionary<Type, Func<ulong, (Width Dimensions, Width Coordinates)>> WidthsByType = new()
	{
		[typeof(byte)] = WidthsFor<byte>,
		[typeof(sbyte)] = WidthsFor<sbyte>,
		[typeof(short)] = WidthsFor<short>,
		[typeof(ushort)] = WidthsFor<ushort>,
		[typeof(int)] = WidthsFor<int>,
		[typeof(uint)] = WidthsFor<uint>,
		[typeof(long)] = WidthsFor<long>,
		[typeof(ulong)] = WidthsFor<ulong>,
	};

	private static (Width, Width) WidthsFor<T>(ulong value)
		where T : struct, IBinaryInteger<T>
	{
		var typedValue = T.CreateChecked(value);
		var dimensions = WidthHelper.GetDimensionsWidth<Dimensions<T>, T>(
			GeometryFactory.Dimensions(typedValue, typedValue, typedValue));
		var coordinates = WidthHelper.GetCoordinatesWidth<Coordinates<T>, T>(
			GeometryFactory.Coordinates(typedValue, typedValue, typedValue));
		return (dimensions, coordinates);
	}

	// Width is internal, so the boxed expected width rides the row as object.
	[Theory]
	[ClassData(typeof(WidthBoundaryByTypeProvider))]
	public void GetWidth_Returns_Expected_For_Type_At_Boundary(Type numericType, ulong value, object expectedValue)
	{
		var expected = (Width)expectedValue;
		var (dimensions, coordinates) = WidthsByType[numericType](value);

		dimensions.ShouldBe(expected);
		coordinates.ShouldBe(expected);
	}

	// Coordinates allow zero, dimensions do not, so (0, 0, 0) is the lowest valid coordinate.
	[Fact]
	public void GetCoordinatesWidth_Returns_Eight_When_All_Coordinates_Are_Zero()
	{
		var coordinates = GeometryFactory.Coordinates(0, 0, 0);

		WidthHelper.GetCoordinatesWidth<Coordinates<int>, int>(coordinates).ShouldBe(Width.Eight);
	}
}
