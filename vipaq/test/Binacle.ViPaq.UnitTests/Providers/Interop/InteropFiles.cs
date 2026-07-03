namespace Binacle.ViPaq.UnitTests.Providers;

// The interop vector file names in one place, as VectorReader takes them (the on-disk slash path, same as
// the TS readVectors). C# names the same three files from several spots — the shared loader, each provider,
// the integrity test — so they live here instead of being retyped. Mirrors the TS artifactFiles list.
internal static class InteropFiles
{
	public const string Input = "interop/input.json";
	public const string CSharpArtifact = "interop/artifact-cs.json";
	public const string TypeScriptArtifact = "interop/artifact-ts.json";
}
