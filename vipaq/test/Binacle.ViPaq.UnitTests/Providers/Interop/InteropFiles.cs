namespace Binacle.ViPaq.UnitTests.Providers;

// Which compression a producer applied to an interop artifact. The wire has no codec field, so Deflate and Gzip
// share the same 'comp' header - the codec is known from the file name, not from the bytes (PROTOCOL.md §6).
internal enum ArtifactCodec { Raw, Deflate, Gzip }

// The interop vector file names, as VectorReader takes them (the on-disk slash path, same as the TS
// readVectors). input.json is shared; each producer has its own folder holding one file per codec:
// interop/<lang>/<codec>.json. Mirrors the TS artifactFiles list.
internal static class InteropFiles
{
	public const string Input = "interop/input.json";

	public const string CSharp = "cs";
	public const string TypeScript = "ts";

	public static readonly ArtifactCodec[] Codecs = [ArtifactCodec.Raw, ArtifactCodec.Deflate, ArtifactCodec.Gzip];

	// lang is "cs" or "ts" (a folder); the file is the lower-cased codec name (raw/deflate/gzip).
	public static string Artifact(string lang, ArtifactCodec codec)
		=> $"interop/{lang}/{codec.ToString().ToLowerInvariant()}.json";

	// Every artifact file: both producers × all three codecs.
	public static IEnumerable<string> All()
		=> from lang in new[] { CSharp, TypeScript }
			from codec in Codecs
			select Artifact(lang, codec);
}
