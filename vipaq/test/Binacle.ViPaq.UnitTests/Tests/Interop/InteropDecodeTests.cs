using Binacle.ViPaq.Compression;
using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Cross-language interop decode. Every artifact, from either producer and in every codec, must deserialize back
// to the exact input it was made from. The TS suite runs the mirror.
//
// Compressed blobs are never byte-compared across languages: DeflateStream and CompressionStream can emit
// different valid streams, so the only contract is decode-to-input. Bytes 0 and 1 are still pinned, so a blob
// that silently stayed uncompressed, or picked the wrong widths, fails before the round-trip check.
[Trait("Result Tests", "Ensures results are as expected")]
public class InteropDecodeTests
{
	[Theory]
	[MemberData(nameof(CSharpArtifacts.Keys), MemberType = typeof(CSharpArtifacts))]
	public void CSharp_Artifact_Decodes_To_Its_Input(string key)
		=> AssertDecodes(CSharpArtifacts.Get(key));

	[Theory]
	[MemberData(nameof(TypeScriptArtifacts.Keys), MemberType = typeof(TypeScriptArtifacts))]
	public void TypeScript_Artifact_Decodes_To_Its_Input(string key)
		=> AssertDecodes(TypeScriptArtifacts.Get(key));

	private static void AssertDecodes(InteropVectors.ArtifactCase artifact)
	{
		// The two header bytes confirm the blob really is what it claims — compression flag, layout, all widths.
		Header.FromBytes(artifact.Bytes[0], artifact.Bytes[1]).ShouldBe(artifact.ExpectedHeader);

		ICompressionCodec codec = artifact.Codec switch
		{
			ArtifactCodec.Deflate => new DeflateCodec(),
			ArtifactCodec.Gzip => new GzipCodec(),
			_ => new NoOpCodec(),
		};
		var expected = new BinContents<long>(artifact.Input.Bin, artifact.Input.Items);
		var actual = ProtocolTestingFixture.DecodeWith<long>(artifact.Bytes, codec);

		BinContents.AssertSame(expected, actual);
	}
}
