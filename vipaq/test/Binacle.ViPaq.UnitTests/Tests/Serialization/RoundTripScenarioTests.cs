using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Shared round-trip scenarios: serialize a (bin, items) input, pin the header byte, then deserialize and
// check the items come back unchanged. The header check makes this stronger than plain round-trip
// equality — a serializer that picked the wrong widths or compression flag would still round-trip, but
// it fails the ExpectedEncodingInfo assertion.
[Trait("Result Tests", "Ensures results are as expected")]
public class RoundTripScenarioTests
{
	[Theory]
	[MemberData(nameof(RoundTripProvider.Names), MemberType = typeof(RoundTripProvider))]
	public void Serializes_With_Expected_Header_And_Round_Trips(string name)
	{
		var scenario = RoundTripProvider.Get(name);

		var data = ViPaqSerializer.Serialize<Bin<long>, Item<long>, long>(scenario.Bin, scenario.Items);

		// byte 0 pins Version and all three bit sizes at once.
		EncodingInfoHelper.FromByte(data[0]).ShouldBe(scenario.ExpectedEncodingInfo);

		SerializationTestingFixture.AssertDeserializesTo(data, scenario.Bin, scenario.Items);
	}
}
