using Binacle.ViPaq.Compression;
using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// Cross-language interop decode — the whole point of the interop vectors. Every artifact, from either producer
// (artifact-cs.*.json and artifact-ts.*.json) and in every codec (raw/deflate/gzip), must deserialize back to the
// exact input it was made from. So the .NET deserializer reads C#'s own output AND the TS output, in all three
// codecs; the TS suite does the mirror. One method per producer, keyed "<codec>/<name>".
//
// Compressed blobs are never byte-compared across languages — DeflateStream and CompressionStream can emit
// different valid streams. The only contract is decode-to-input. byte 0/1 is still pinned, so a blob that silently
// stayed uncompressed (or picked the wrong widths) fails before the round-trip check. Every artifact decodes the
// same way — ProtocolEncoder + the codec named by the file (raw = NoOp, which leaves the body untouched) — so
// there is no special case per codec.
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
		SerializationTestingFixture.AssertCodecDecodesTo(artifact.Bytes, codec, artifact.Input.Bin, artifact.Input.Items);
	}
}
