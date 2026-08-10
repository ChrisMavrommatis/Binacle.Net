using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Shared round-trip scenarios: encode a (bin, items) input under the scenario's header, pin the header bytes,
// then decode and check the items come back unchanged. The header check makes this stronger than plain
// round-trip equality — a codec that wrote the wrong widths, layout or compression flag would still round-trip
// but fails the header-bytes assertion.
//
// These drive ProtocolEncoder (through the fixture), not ViPaqSerializer, so the scenario's header is an input:
// that is what lets a scenario be columnar or wider than narrowest. ViPaqSerializer derives the widths itself,
// so those scenarios are unreachable through it. Every scenario is uncompressed, because the fixture's NoOp
// codec is what keeps the header-bytes pin meaningful (PROTOCOL.md §6.1 forbids comparing real compressed
// bytes).
[Trait("Result Tests", "Ensures results are as expected")]
public class RoundTripScenarioTests
{
	[Theory]
	[MemberData(nameof(RoundTripProvider.Names), MemberType = typeof(RoundTripProvider))]
	public void Encodes_With_Expected_Header_And_Round_Trips(string name)
	{
		var scenario = RoundTripProvider.Get(name);

		var data = ProtocolTestingFixture.Encode(scenario.ExpectedHeader, scenario.Bin, scenario.Items);

		// A cheap guard that Encode actually wrote the header it was handed, and that ToBytes/FromBytes agree on
		// these bytes. It is NOT an independent oracle — the header is the encode input, so it echoes back by
		// construction; the exhaustive header-byte packing lives in HeaderBytesTests. The real coverage here is
		// the line below: the blob decodes back to the same bin and items under a forced (columnar or wider)
		// header, which HeaderBytesTests does not exercise.
		Header.FromBytes(data[0], data[1]).ShouldBe(scenario.ExpectedHeader);

		var expected = new BinContents<long>(scenario.Bin, scenario.Items);
		var actual = ProtocolTestingFixture.Deserialize<long>(data);

		BinContents.AssertSame(expected, actual);
	}
}
