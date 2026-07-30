using Binacle.ViPaq.Helpers;
using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Shared width-selection vectors, split by kind. Dimensions and coordinates use identical width math, so
// each picker must return the expected width; the two sets together cover every bucket, pinning the pickers
// so they can't drift.
//
// Folded in from the deleted BitSizeHelperTests: that test pinned the Eight<->Sixteen boundary (255 -> Eight,
// 256 -> Sixteen) inline; those rows now live in width-selection.json. Its 32/64-bit boundary cases are gone
// with those tiers.
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
