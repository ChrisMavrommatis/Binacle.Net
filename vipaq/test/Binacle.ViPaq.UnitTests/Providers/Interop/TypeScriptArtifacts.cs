namespace Binacle.ViPaq.UnitTests.Providers;

// The TypeScript producer's artifact file. Mirror of CSharpArtifacts — same shared loader, its own file.
internal static class TypeScriptArtifacts
{
	private static readonly Dictionary<string, InteropVectors.ArtifactCase> artifacts;
	static TypeScriptArtifacts()
	{
		artifacts = InteropVectors.Load(InteropFiles.TypeScriptArtifact);
	}

	public static IEnumerable<object[]> Names
		=> artifacts.Keys.Select(name => new object[] { name });

	public static InteropVectors.ArtifactCase Get(string name)
		=> artifacts[name];
}
