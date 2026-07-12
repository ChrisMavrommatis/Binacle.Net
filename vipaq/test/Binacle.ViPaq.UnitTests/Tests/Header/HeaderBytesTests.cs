using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// The two header bytes. ToBytes packs the version, the two form bits and the three section widths into two
// bytes; FromBytes unpacks them back. The shared header-bytes.json (every combo, keyed by its header
// notation) grades both directions and the round trip between them.
[Trait("Result Tests", "Ensures results are as expected")]
public class HeaderBytesTests
{
	[Theory]
	[MemberData(nameof(HeaderBytesProvider.Keys), MemberType = typeof(HeaderBytesProvider))]
	public void ToBytes_Returns_Correct_Bytes(string key)
	{
		var scenario = HeaderBytesProvider.Get(key);

		var bytes = new byte[Header.ByteCount];
		scenario.Header.ToBytes(bytes);

		bytes.ShouldBe(scenario.Bytes);
	}

	[Theory]
	[MemberData(nameof(HeaderBytesProvider.Keys), MemberType = typeof(HeaderBytesProvider))]
	public void FromBytes_Returns_Correct_Header(string key)
	{
		var scenario = HeaderBytesProvider.Get(key);

		Header.FromBytes(scenario.Bytes[0], scenario.Bytes[1]).ShouldBe(scenario.Header);
	}

	[Theory]
	[MemberData(nameof(HeaderBytesProvider.Keys), MemberType = typeof(HeaderBytesProvider))]
	public void ToBytes_Then_FromBytes_Returns_Original(string key)
	{
		var scenario = HeaderBytesProvider.Get(key);

		var bytes = new byte[Header.ByteCount];
		scenario.Header.ToBytes(bytes);
		bytes.ShouldBe(scenario.Bytes);

		Header.FromBytes(bytes[0], bytes[1]).ShouldBe(scenario.Header);
	}
}
