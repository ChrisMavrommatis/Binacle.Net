namespace Binacle.ViPaq.UnitTests.Providers;

// The TypeScript producer's artifacts — the interop/ts folder, mirror of CSharpArtifacts. Same shared loader, same
// "<codec>/<name>" keying.
internal static class TypeScriptArtifacts
{
	private static readonly Dictionary<string, InteropVectors.ArtifactCase> byKey = new();
	static TypeScriptArtifacts()
	{
		foreach (var codec in InteropFiles.Codecs)
		foreach (var (name, artifact) in InteropVectors.Load(InteropFiles.Artifact(InteropFiles.TypeScript, codec), codec))
			byKey[$"{codec}/{name}"] = artifact;
	}

	public static IEnumerable<object[]> Keys
		=> byKey.Keys.Select(key => new object[] { key });

	public static InteropVectors.ArtifactCase Get(string key)
		=> byKey[key];
}
