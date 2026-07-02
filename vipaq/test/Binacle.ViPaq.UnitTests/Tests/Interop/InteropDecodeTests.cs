using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Cross-language interop, C# side. A blob the C# tool produced must deserialize back to the exact input
// it was made from. Right now this is the C#->C# corner of the matrix (own round-trip); the
// cross-language corner (decode TS's artifact-ts.json) joins once the TS generator exists.
//
// Compressed blobs are never byte-compared across languages — GZipStream (C#) and CompressionStream
// (Node) emit different valid gzip for the same input. The only stable contract is decode-to-input,
// which is exactly what this asserts. byte 0 is still pinned, so a blob that silently stayed
// uncompressed (or picked the wrong widths) fails before the round-trip check.
[Trait("Result Tests", "Ensures results are as expected")]
public class InteropDecodeTests
{
	[Theory]
	[MemberData(nameof(InteropProvider.CSharpNames), MemberType = typeof(InteropProvider))]
	public void CSharp_Artifact_Decodes_To_Its_Input(string name)
	{
		var artifact = InteropProvider.GetCSharpArtifact(name);
		var input = InteropProvider.GetInput(name);

		// byte 0 confirms the blob really is what it claims — compression flag plus all three widths.
		EncodingInfoHelper.FromByte(artifact.Bytes[0]).ShouldBe(artifact.ExpectedEncodingInfo);

		SerializationTestingFixture.AssertDeserializesTo(artifact.Bytes, input.Bin, input.Items);
	}
}
