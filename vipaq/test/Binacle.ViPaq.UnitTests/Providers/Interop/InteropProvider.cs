using Binacle.ViPaq.UnitTests.Models;
using Version = Binacle.ViPaq.Version;

namespace Binacle.ViPaq.UnitTests.Providers;

// NOTE (needs simplifying — see plan): this provider is doing too much. It loads input.json plus both
// artifact files and serves three different checks (decode, integrity, byte-identity) off a nested
// dictionary. Next session: either flatten it to one simple shape or split it into two providers
// (decode vs. integrity/byte-identity). Left as-is (green) on purpose — do not refactor piecemeal.
//
// The interop artifacts (interop/artifact-cs.json, interop/artifact-ts.json) both serialize the shared
// input.json; each blob must deserialize back to it.
internal static class InteropProvider
{
	public sealed record Input(Bin<long> Bin, Item<long>[] Items);

	public sealed record ArtifactCase(string Producer, string Name, EncodingInfo ExpectedEncodingInfo, byte[] Bytes, Input Input);

	private const string CSharpFile = "interop.artifact-cs.json";
	private const string TypeScriptFile = "interop.artifact-ts.json";

	private static readonly Dictionary<string, Input> inputs;
	// file name -> (scenario Name -> case)
	private static readonly Dictionary<string, Dictionary<string, ArtifactCase>> artifactsByFile;

	static InteropProvider()
	{
		inputs = new Dictionary<string, Input>();
		foreach (var vector in VectorReader.Read<InputVector>("interop.input.json"))
		{
			inputs.Add(vector.Name, new Input(
				VectorParser.ParseBin(vector.Bin),
				VectorParser.ParseItems(vector.Items).ToArray()));
		}

		artifactsByFile = new Dictionary<string, Dictionary<string, ArtifactCase>>
		{
			[CSharpFile] = LoadArtifacts(CSharpFile),
			[TypeScriptFile] = LoadArtifacts(TypeScriptFile),
		};
	}

	// Reads an artifact file into a Name-keyed dictionary. Keying by Name rejects duplicate names in the
	// file; whether the names line up with input.json is checked by the integrity test.
	private static Dictionary<string, ArtifactCase> LoadArtifacts(string fileName)
	{
		var artifacts = new Dictionary<string, ArtifactCase>();
		foreach (var vector in VectorReader.Read<ArtifactVector>(fileName))
		{
			artifacts.Add(vector.Name, new ArtifactCase(
				vector.Producer,
				vector.Name,
				VectorParser.ParseEncodingInfo(vector.EncodingInfo),
				Convert.FromBase64String(vector.Base64),
				inputs[vector.Name]));
		}

		return artifacts;
	}

	// --- decode: one row per (producer, scenario), across both files ---
	public static IEnumerable<object[]> DecodeCases
		=> artifactsByFile.Values
			.SelectMany(byName => byName.Values)
			.Select(artifact => new object[] { artifact.Producer, artifact.Name });

	public static ArtifactCase Get(string producer, string name)
		=> artifactsByFile.Values
			.SelectMany(byName => byName.Values)
			.Single(artifact => artifact.Producer == producer && artifact.Name == name);

	// --- integrity: each artifact file must cover exactly the input scenarios ---
	public static IEnumerable<object[]> ArtifactFiles
		=> artifactsByFile.Keys.Select(fileName => new object[] { fileName });

	public static IReadOnlyCollection<string> InputNames
		=> inputs.Keys;

	public static IReadOnlyCollection<string> ArtifactNames(string fileName)
		=> artifactsByFile[fileName].Keys;

	// --- byte-identity: uncompressed blobs must match byte-for-byte across producers ---
	public static IEnumerable<object[]> UncompressedNames
		=> artifactsByFile[CSharpFile].Values
			.Where(artifact => artifact.ExpectedEncodingInfo.Version == Version.Uncompressed)
			.Select(artifact => new object[] { artifact.Name });

	public static byte[] CSharpBytes(string name) => artifactsByFile[CSharpFile][name].Bytes;
	public static byte[] TypeScriptBytes(string name) => artifactsByFile[TypeScriptFile][name].Bytes;

	// Raw interop/input.json row: a (bin, items) input in the shared compact-string form.
	private sealed class InputVector
	{
		public string Name { get; set; } = "";
		public string Bin { get; set; } = "";
		public string[] Items { get; set; } = [];
	}

	// Raw artifact-*.json row: a base64 blob plus the header string it must decode to.
	private sealed class ArtifactVector
	{
		public string Name { get; set; } = "";
		public string Producer { get; set; } = "";
		public string EncodingInfo { get; set; } = "";
		public string Base64 { get; set; } = "";
	}
}
