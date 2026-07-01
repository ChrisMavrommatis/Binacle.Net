using Binacle.ViPaq.Helpers;
using Binacle.ViPaq.UnitTests.Models;
using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Shared picker-reject vectors. Kind routes the case to the right picker; the picker must throw an
// ArgumentOutOfRangeException whose ParamName names the offending field (Length / Width / Height for
// dimensions, X / Y / Z for coordinates).
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class BitSizeInvalidTests
{
	[Theory]
	[MemberData(nameof(BitSizeInvalidProvider.Names), MemberType = typeof(BitSizeInvalidProvider))]
	public void Picker_Throws_For_Offending_Field(string name)
	{
		var scenario = BitSizeInvalidProvider.Get(name);

		var exception = scenario.Kind == BitSizeInvalidProvider.BitSizeKind.Dimensions
			? Should.Throw<ArgumentOutOfRangeException>(() =>
				BitSizeHelper.GetDimensionsBitSize<Dimensions<long>, long>(scenario.Dimensions))
			: Should.Throw<ArgumentOutOfRangeException>(() =>
				BitSizeHelper.GetCoordinatesBitSize<Coordinates<long>, long>(scenario.Coordinates));

		exception.ParamName.ShouldBe(scenario.Field);
	}
}
