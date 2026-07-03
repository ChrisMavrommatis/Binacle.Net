using Binacle.ViPaq.Helpers;
using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Shared picker-reject vectors, split by kind. Each picker must throw an ArgumentOutOfRangeException whose
// ParamName names the offending field (Length / Width / Height for dimensions, X / Y / Z for coordinates).
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class BitSizeInvalidTests
{
	[Theory]
	[MemberData(nameof(BitSizeInvalidProvider.DimensionNames), MemberType = typeof(BitSizeInvalidProvider))]
	public void Dimensions_Picker_Throws_For_Offending_Field(string name)
	{
		var scenario = BitSizeInvalidProvider.Dimension(name);

		var exception = Should.Throw<ArgumentOutOfRangeException>(() =>
			BitSizeHelper.GetDimensionsBitSize<Dimensions<long>, long>(scenario.Value));

		exception.ParamName.ShouldBe(scenario.Field);
	}

	[Theory]
	[MemberData(nameof(BitSizeInvalidProvider.CoordinateNames), MemberType = typeof(BitSizeInvalidProvider))]
	public void Coordinates_Picker_Throws_For_Offending_Field(string name)
	{
		var scenario = BitSizeInvalidProvider.Coordinate(name);

		var exception = Should.Throw<ArgumentOutOfRangeException>(() =>
			BitSizeHelper.GetCoordinatesBitSize<Coordinates<long>, long>(scenario.Value));

		exception.ParamName.ShouldBe(scenario.Field);
	}
}
