using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Cross-language interop decode. Every interop artifact — from either producer (artifact-cs.json and
// artifact-ts.json) — must deserialize back to the exact input it was made from. So this decodes C#'s own
// output AND the TS output through the .NET 10 deserializer; the TS suite does the mirror.
//
// Compressed blobs are never byte-compared across languages — GZipStream and CompressionStream can emit
// different valid gzip. The only contract is decode-to-input. byte 0 is still pinned, so a blob that
// silently stayed uncompressed (or picked the wrong widths) fails before the round-trip check.
[Trait("Result Tests", "Ensures results are as expected")]
public class InteropDecodeTests
{
	[Theory]
	[MemberData(nameof(InteropProvider.DecodeCases), MemberType = typeof(InteropProvider))]
	public void Artifact_Decodes_To_Its_Input(string producer, string name)
	{
		var artifact = InteropProvider.Get(producer, name);

		// byte 0 confirms the blob really is what it claims — compression flag plus all three widths.
		EncodingInfoHelper.FromByte(artifact.Bytes[0]).ShouldBe(artifact.ExpectedEncodingInfo);

		SerializationTestingFixture.AssertDeserializesTo(artifact.Bytes, artifact.Input.Bin, artifact.Input.Items);
	}
}
