using Binacle.ViPaq.UnitTests.Models;

namespace Binacle.ViPaq.UnitTests.Providers;

// Cross-language interop artifacts (vipaq/test-vectors/interop/). input.json is the shared (bin, items)
// both language generators serialize; artifact-cs.json holds what the C# tool produced — the base64
// blob plus the header it must decode to. Inputs and artifacts are joined by Name: the decode test looks
// up an artifact's expected (bin, items) from input.json by the same Name. TS's artifact-ts.json joins
// here once the TS generator exists.
internal static class InteropProvider
{
	public sealed record Input(Bin<long> Bin, Item<long>[] Items);

	public sealed record ArtifactCase(string Producer, EncodingInfo ExpectedEncodingInfo, byte[] Bytes);

	private static readonly Dictionary<string, Input> inputs;
	private static readonly Dictionary<string, ArtifactCase> csharpArtifacts;

	// Static constructor: the vectors load once, on first access to this provider.
	static InteropProvider()
	{
		inputs = new Dictionary<string, Input>();
		foreach (var vector in VectorReader.Read<InputVector>("interop.input.json"))
		{
			inputs.Add(vector.Name, new Input(
				VectorParser.ParseBin(vector.Bin),
				VectorParser.ParseItems(vector.Items).ToArray()));
		}

		csharpArtifacts = LoadArtifacts("interop.artifact-cs.json");
	}

	// Reads an artifact file into a Name-keyed dictionary. Keying by Name rejects duplicate names in the
	// file; whether the names line up with input.json is checked separately by InteropIntegrityTests.
	private static Dictionary<string, ArtifactCase> LoadArtifacts(string fileName)
	{
		var artifacts = new Dictionary<string, ArtifactCase>();
		foreach (var vector in VectorReader.Read<ArtifactVector>(fileName))
		{
			artifacts.Add(vector.Name, new ArtifactCase(
				vector.Producer,
				VectorParser.ParseEncodingInfo(vector.EncodingInfo),
				Convert.FromBase64String(vector.Base64)));
		}

		return artifacts;
	}

	public static IEnumerable<object[]> CSharpNames
		=> csharpArtifacts.Keys.Select(name => new object[] { name });

	public static ArtifactCase GetCSharpArtifact(string name)
		=> csharpArtifacts[name];

	public static Input GetInput(string name)
		=> inputs[name];

	// Name sets for the integrity check — input.json and each artifact file must cover the same scenarios.
	public static IReadOnlyCollection<string> InputNames
		=> inputs.Keys;

	public static IReadOnlyCollection<string> CSharpArtifactNames
		=> csharpArtifacts.Keys;

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
