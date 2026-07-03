using Version = Binacle.ViPaq.Version;

namespace Binacle.ViPaq.UnitTests.Providers;

// The C# producer's artifact file. Each provider IS its file — CSharpArtifacts reads artifact-cs.json,
// TypeScriptArtifacts reads artifact-ts.json, both through the shared InteropVectors.Load. Decode and
// integrity consume whichever they need on its own; only byte-identity touches both.
internal static class CSharpArtifacts
{
	private static readonly Dictionary<string, InteropVectors.ArtifactCase> artifacts;
	static CSharpArtifacts()
	{
		artifacts = InteropVectors.Load(InteropFiles.CSharpArtifact);
	}

	public static IEnumerable<object[]> Names
		=> artifacts.Keys.Select(name => new object[] { name });

	// Row source for the byte-identity test: only the blobs the spec fully determines (no gzip engine).
	public static IEnumerable<object[]> UncompressedNames
		=> artifacts.Values
			.Where(artifact => artifact.ExpectedEncodingInfo.Version == Version.Uncompressed)
			.Select(artifact => new object[] { artifact.Name });

	public static InteropVectors.ArtifactCase Get(string name)
		=> artifacts[name];
}
