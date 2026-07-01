using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// The header byte. ToByte packs the version and the three section sizes into one byte, FromByte unpacks
// it back. The shared encoding-info-bytes.json (every combo, keyed by its EncodingInfo string) grades
// both directions and the round trip between them.
[Trait("Result Tests", "Ensures results are as expected")]
public class EncodingInfoByteTests
{
	[Theory]
	[MemberData(nameof(EncodingInfoByteProvider.Keys), MemberType = typeof(EncodingInfoByteProvider))]
	public void ToByte_Returns_Correct_Byte(string key)
	{
		var scenario = EncodingInfoByteProvider.Get(key);

		EncodingInfoHelper.ToByte(scenario.Info).ShouldBe(scenario.Byte);
	}

	[Theory]
	[MemberData(nameof(EncodingInfoByteProvider.Keys), MemberType = typeof(EncodingInfoByteProvider))]
	public void FromByte_Returns_Correct_EncodingInfo(string key)
	{
		var scenario = EncodingInfoByteProvider.Get(key);

		EncodingInfoHelper.FromByte(scenario.Byte).ShouldBe(scenario.Info);
	}

	[Theory]
	[MemberData(nameof(EncodingInfoByteProvider.Keys), MemberType = typeof(EncodingInfoByteProvider))]
	public void ToByte_Then_FromByte_Returns_Original(string key)
	{
		var scenario = EncodingInfoByteProvider.Get(key);

		var asByte = EncodingInfoHelper.ToByte(scenario.Info);
		asByte.ShouldBe(scenario.Byte);

		EncodingInfoHelper.FromByte(asByte).ShouldBe(scenario.Info);
	}
}
