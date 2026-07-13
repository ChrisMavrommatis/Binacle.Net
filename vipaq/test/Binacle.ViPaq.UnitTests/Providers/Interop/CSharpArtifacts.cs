namespace Binacle.ViPaq.UnitTests.Providers;

// The C# producer's artifacts — the interop/cs folder, one file per codec (raw/deflate/gzip), all loaded through
// the shared InteropVectors.Load. Keyed "<codec>/<name>" because the same scenario names appear in every codec
// file. The decode test walks these keys; TypeScriptArtifacts is the mirror for the interop/ts folder.
internal static class CSharpArtifacts
{
	private static readonly Dictionary<string, InteropVectors.ArtifactCase> byKey = new();
	static CSharpArtifacts()
	{
		foreach (var codec in InteropFiles.Codecs)
		foreach (var (name, artifact) in InteropVectors.Load(InteropFiles.Artifact(InteropFiles.CSharp, codec), codec))
			byKey[$"{codec}/{name}"] = artifact;
	}

	public static IEnumerable<object[]> Keys
		=> byKey.Keys.Select(key => new object[] { key });

	public static InteropVectors.ArtifactCase Get(string key)
		=> byKey[key];
}
