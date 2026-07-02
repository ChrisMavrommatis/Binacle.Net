using Binacle.ViPaq.UnitTests.Providers;

namespace Binacle.ViPaq.UnitTests;

// The one safe byte comparison across producers. An UNCOMPRESSED blob is fully determined by the spec —
// no gzip engine involved — so the C# and TS generators MUST emit byte-identical bytes for it. (Compressed
// blobs are the opposite: their gzip bytes are not reproducible across engines/runtimes, so they are never
// byte-compared — see PROTOCOL.md §6.) This is a language-agnostic fact about the committed files, so it
// lives on one side only.
[Trait("Result Tests", "Ensures results are as expected")]
public class InteropByteIdentityTests
{
	[Theory]
	[MemberData(nameof(InteropProvider.UncompressedNames), MemberType = typeof(InteropProvider))]
	public void Uncompressed_Blob_Is_Byte_Identical_Across_Producers(string name)
	{
		InteropProvider.TypeScriptBytes(name).ShouldBe(InteropProvider.CSharpBytes(name));
	}
}
