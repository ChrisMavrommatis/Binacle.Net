using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Shared round-trip scenarios: encode under the scenario's header, pin the header bytes, then decode and check
// the items come back unchanged. The header check is what makes this stronger than plain round-trip equality -
// a codec writing the wrong widths, layout or compression flag would still round-trip.
//
// These drive ProtocolEncoder, not ViPaqSerializer, so the header is an input and a scenario can be columnar or
// wider than narrowest. Every scenario is uncompressed, because PROTOCOL.md §6.1 forbids comparing real
// compressed bytes.
[Trait("Result Tests", "Ensures results are as expected")]
public class RoundTripScenarioTests
{
	[Theory]
	[MemberData(nameof(RoundTripProvider.Names), MemberType = typeof(RoundTripProvider))]
	public void Encodes_With_Expected_Header_And_Round_Trips(string name)
	{
		var scenario = RoundTripProvider.Get(name);

		var data = ProtocolTestingFixture.Encode(scenario.ExpectedHeader, scenario.Bin, scenario.Items);

		// NOT an independent oracle: the header is the encode input, so it echoes back by construction. The
		// exhaustive header-byte packing lives in HeaderBytesTests; the real coverage here is the line below.
		Header.FromBytes(data[0], data[1]).ShouldBe(scenario.ExpectedHeader);

		var expected = new BinContents<long>(scenario.Bin, scenario.Items);
		var actual = ProtocolTestingFixture.Deserialize<long>(data);

		BinContents.AssertSame(expected, actual);
	}
}
