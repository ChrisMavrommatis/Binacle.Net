using Binacle.ViPaq.Helpers;

namespace Binacle.ViPaq.UnitTests;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class WidthHelperBehaviorTests
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
	public void GetDimensionsWidth_Throws_ArgumentOutOfRangeException_ForParamName(
		int length,
		int width,
		int height,
		string expectedThrownParamName)
	{
		var dimensions = GeometryFactory.Dimensions(length, width, height);
		var exception =
			Should.Throw<ArgumentOutOfRangeException>(() =>
				WidthHelper.GetDimensionsWidth<Dimensions<int>, int>(dimensions)
			);

		exception.ParamName.ShouldBe(expectedThrownParamName);
	}

	[Theory]
	[InlineData(-1, 0, 0, nameof(Coordinates<int>.X))]
	[InlineData(0, -1, 0, nameof(Coordinates<int>.Y))]
	[InlineData(0, 0, -1, nameof(Coordinates<int>.Z))]
	public void GetCoordinatesWidth_Throws_ArgumentOutOfRangeException_ForParamName(
		int x,
		int y,
		int z,
		string expectedThrownParamName)
	{
		var coordinates = GeometryFactory.Coordinates(x, y, z);
		var exception = Should.Throw<ArgumentOutOfRangeException>(() =>
			WidthHelper.GetCoordinatesWidth<Coordinates<int>, int>(coordinates)
		);

		exception.ParamName.ShouldBe(expectedThrownParamName);
	}
}
