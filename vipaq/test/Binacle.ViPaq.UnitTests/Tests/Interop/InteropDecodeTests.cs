using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Cross-language interop decode. Every interop artifact — from either producer (artifact-cs.json and
// artifact-ts.json) — must deserialize back to the exact input it was made from. So the .NET 10
// deserializer reads C#'s own output AND the TS output; the TS suite does the mirror. One method per
// producer, each fed by that producer's provider.
//
// Compressed blobs are never byte-compared across languages — GZipStream and CompressionStream can emit
// different valid gzip. The only contract is decode-to-input. byte 0 is still pinned, so a blob that
// silently stayed uncompressed (or picked the wrong widths) fails before the round-trip check.
[Trait("Result Tests", "Ensures results are as expected")]
public class InteropDecodeTests
{
	[Theory]
	[MemberData(nameof(CSharpArtifacts.Names), MemberType = typeof(CSharpArtifacts))]
	public void CSharp_Artifact_Decodes_To_Its_Input(string name)
		=> AssertDecodes(CSharpArtifacts.Get(name));

	[Theory]
	[MemberData(nameof(TypeScriptArtifacts.Names), MemberType = typeof(TypeScriptArtifacts))]
	public void TypeScript_Artifact_Decodes_To_Its_Input(string name)
		=> AssertDecodes(TypeScriptArtifacts.Get(name));

	private static void AssertDecodes(InteropVectors.ArtifactCase artifact)
	{
		// byte 0 confirms the blob really is what it claims — compression flag plus all three widths.
		EncodingInfoHelper.FromByte(artifact.Bytes[0]).ShouldBe(artifact.ExpectedEncodingInfo);

		SerializationTestingFixture.AssertDeserializesTo(artifact.Bytes, artifact.Input.Bin, artifact.Input.Items);
	}
}
