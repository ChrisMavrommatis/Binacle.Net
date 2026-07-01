using Binacle.ViPaq.Helpers;
using Binacle.ViPaq.UnitTests.Models;
using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Shared width-selection vectors. The same triple runs through both pickers — dimensions and
// coordinates use identical width math, so both must return the expected width. That pins the two
// pickers together so they can't drift.
[Trait("Result Tests", "Ensures results are as expected")]
public class BitSizeSelectionTests
{
	[Theory]
	[MemberData(nameof(BitSizeSelectionProvider.Names), MemberType = typeof(BitSizeSelectionProvider))]
	public void Picks_Expected_Width_For_Dimensions_And_Coordinates(string name)
	{
		var scenario = BitSizeSelectionProvider.Get(name);

		BitSizeHelper.GetDimensionsBitSize<Dimensions<long>, long>(scenario.Dimensions).ShouldBe(scenario.Expected);
		BitSizeHelper.GetCoordinatesBitSize<Coordinates<long>, long>(scenario.Coordinates).ShouldBe(scenario.Expected);
	}
}
