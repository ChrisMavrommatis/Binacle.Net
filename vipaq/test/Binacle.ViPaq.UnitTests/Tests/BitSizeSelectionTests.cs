using Binacle.ViPaq.Helpers;
using Binacle.ViPaq.UnitTests.Models;
using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Shared width-selection vectors, split by kind. Dimensions and coordinates use identical width math, so
// each picker must return the expected width; the two sets together cover every bucket, pinning the pickers
// so they can't drift.
[Trait("Result Tests", "Ensures results are as expected")]
public class BitSizeSelectionTests
{
	[Theory]
	[MemberData(nameof(BitSizeSelectionProvider.DimensionNames), MemberType = typeof(BitSizeSelectionProvider))]
	public void Picks_Expected_Width_For_Dimensions(string name)
	{
		var scenario = BitSizeSelectionProvider.Dimension(name);

		BitSizeHelper.GetDimensionsBitSize<Dimensions<long>, long>(scenario.Value).ShouldBe(scenario.Expected);
	}

	[Theory]
	[MemberData(nameof(BitSizeSelectionProvider.CoordinateNames), MemberType = typeof(BitSizeSelectionProvider))]
	public void Picks_Expected_Width_For_Coordinates(string name)
	{
		var scenario = BitSizeSelectionProvider.Coordinate(name);

		BitSizeHelper.GetCoordinatesBitSize<Coordinates<long>, long>(scenario.Value).ShouldBe(scenario.Expected);
	}
}
