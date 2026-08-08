
namespace Binacle.ViPaq.UnitTests;

// The public entry point, end to end. Every other serialization test drives ProtocolEncoder directly (so it can
// force a compressed or columnar header, which ViPaqSerializer never can). But that leaves ViPaqSerializer's own
// job untested: choosing a raw, row-major, narrowest header AND round-tripping a real caller through Serialize
// -> Deserialize. These few cases pin exactly that — the one path a real caller actually takes.
//
// ViPaqSerializer only ever writes raw blobs, so these are all raw; the compressed-refusal on the decode side is
// pinned separately in SerializationBehaviorTests. Fields are distinct per item so a wiring bug (one field read
// into another, or items swapped) shows up as a mismatch.
[Trait("Result Tests", "Ensures results are as expected")]
public class ViPaqSerializerTests
{
	[Fact]
	public void Round_Trips_8_Bit_Values_Through_The_Public_Api()
	{
		var expected = new BinContents<int>(
			new Binacle.Geometry.Dimensions<int> { Length = 10, Width = 20, Height = 30 },
			[
				new() { Length = 1, Width = 2, Height = 3, X = 4, Y = 5, Z = 6 },
				new() { Length = 7, Width = 8, Height = 9, X = 10, Y = 11, Z = 12 },
			]);

		var actual = ViPaqSerializerTestingFixture.RoundTrip(expected);

		BinContents.AssertSame(expected, actual);
	}

	[Fact]
	public void Round_Trips_16_Bit_Values_Through_The_Public_Api()
	{
		var expected = new BinContents<int>(
			new Binacle.Geometry.Dimensions<int> { Length = 1000, Width = 2000, Height = 3000 },
			[new() { Length = 300, Width = 400, Height = 500, X = 600, Y = 700, Z = 800 }]);

		var actual = ViPaqSerializerTestingFixture.RoundTrip(expected);

		BinContents.AssertSame(expected, actual);
	}

	[Fact]
	public void Round_Trips_With_No_Items_Through_The_Public_Api()
	{
		var expected = new BinContents<int>(
			new Binacle.Geometry.Dimensions<int> { Length = 10, Width = 20, Height = 30 },
			[]);

		var actual = ViPaqSerializerTestingFixture.RoundTrip(expected);

		BinContents.AssertSame(expected, actual);
	}

}
