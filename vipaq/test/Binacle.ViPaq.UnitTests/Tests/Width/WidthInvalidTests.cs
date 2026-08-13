using Binacle.ViPaq.Helpers;
using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Shared picker-reject vectors, split by kind. Each picker must throw an ArgumentOutOfRangeException whose
// ParamName names the offending field.
//
// There is no saturation: a value above the 65535 ceiling is rejected outright, which is what the
// "exceeds max (65536)" rows in width-invalid.json cover.
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class WidthInvalidTests
{
	[Theory]
	[MemberData(nameof(WidthInvalidProvider.DimensionNames), MemberType = typeof(WidthInvalidProvider))]
	public void Dimensions_Picker_Throws_For_Offending_Field(string name)
	{
		var scenario = WidthInvalidProvider.Dimension(name);

		var exception = Should.Throw<ArgumentOutOfRangeException>(() =>
			WidthHelper.GetDimensionsWidth<Dimensions<long>, long>(scenario.Value));

		exception.ParamName.ShouldBe(scenario.Field);
	}

	[Theory]
	[MemberData(nameof(WidthInvalidProvider.CoordinateNames), MemberType = typeof(WidthInvalidProvider))]
	public void Coordinates_Picker_Throws_For_Offending_Field(string name)
	{
		var scenario = WidthInvalidProvider.Coordinate(name);

		var exception = Should.Throw<ArgumentOutOfRangeException>(() =>
			WidthHelper.GetCoordinatesWidth<Coordinates<long>, long>(scenario.Value));

		exception.ParamName.ShouldBe(scenario.Field);
	}
}
