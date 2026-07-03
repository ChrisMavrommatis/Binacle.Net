namespace Binacle.ViPaq.UnitTests.Providers;

// The interop vector file names in one place, as VectorReader takes them (it adds the "Data." prefix).
// C# names the same three files from several spots — the shared loader, each provider, the integrity
// test — so they live here instead of being retyped. Mirrors the TS artifactFiles list in InteropArtifacts.ts.
internal static class InteropFiles
{
	public const string Input = "interop.input.json";
	public const string CSharpArtifact = "interop.artifact-cs.json";
	public const string TypeScriptArtifact = "interop.artifact-ts.json";
}
