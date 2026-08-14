using Binacle.ViPaq.Helpers;
using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Shared width-selection vectors, split by kind. Dimensions and coordinates use identical width math, and the
// two sets together cover every bucket, including the 255 -> Eight / 256 -> Sixteen boundary.
[Trait("Result Tests", "Ensures results are as expected")]
public class WidthSelectionTests
{
	[Theory]
	[MemberData(nameof(WidthSelectionProvider.DimensionNames), MemberType = typeof(WidthSelectionProvider))]
	public void Picks_Expected_Width_For_Dimensions(string name)
	{
		var scenario = WidthSelectionProvider.Dimension(name);

		WidthHelper.GetDimensionsWidth<Dimensions<long>, long>(scenario.Value).ShouldBe(scenario.Expected);
	}

	[Theory]
	[MemberData(nameof(WidthSelectionProvider.CoordinateNames), MemberType = typeof(WidthSelectionProvider))]
	public void Picks_Expected_Width_For_Coordinates(string name)
	{
		var scenario = WidthSelectionProvider.Coordinate(name);

		WidthHelper.GetCoordinatesWidth<Coordinates<long>, long>(scenario.Value).ShouldBe(scenario.Expected);
	}
}
