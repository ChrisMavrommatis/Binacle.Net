
namespace Binacle.ViPaq.UnitTests.Providers;

// Shared loader for the interop vectors. CSharpArtifacts and TypeScriptArtifacts are thin providers that call
// Load with their own artifact file. xUnit [MemberData] sources must be static and C# static classes cannot
// derive, so this is a shared static helper plus two static providers rather than real inheritance.
//
// input.json holds the shared scenarios; every artifact blob must deserialize back to the input it names.
internal static class InteropVectors
{
	// ExpectedHeader lives on the input, not the artifact: it is producer-independent and spec-determined, so
	// the header pin is a real oracle instead of a value echoed back from the generator's own output.
	public sealed record Input(Header ExpectedHeader, Binacle.Geometry.Dimensions<long> Bin, Binacle.Geometry.Item<long>[] Items);

	// Producer and Codec ride along so the decode test can pick its path: raw through ViPaqSerializer,
	// deflate/gzip through ProtocolEncoder plus the matching codec.
	public sealed record ArtifactCase(string Producer, string Name, ArtifactCodec Codec, Header ExpectedHeader, byte[] Bytes, Input Input);

	// input.json, loaded once and shared by both providers.
	private static readonly Dictionary<string, Input> inputs;
	static InteropVectors()
	{
		inputs = new Dictionary<string, Input>();
		foreach (var vector in VectorReader.Read<InputVector>(InteropFiles.Input))
		{
			var input = new Input(
				VectorParser.ParseHeader(vector.ExpectedHeader),
				VectorParser.ParseBin(vector.Bin),
				VectorParser.ParseItems(vector.Items).ToArray());
			inputs.Add(vector.Name, input);
		}
	}

	public static IReadOnlyCollection<string> InputNames
		=> inputs.Keys;

	// Just the Name of each row, without joining to input.json. The integrity test needs this to report which
	// names differ; going through Load would throw inside a provider's static ctor first and surface as a
	// murky TypeInitializationException.
	public static IReadOnlyList<string> ReadNames(string fileName)
		=> VectorReader.Read<ArtifactVector>(fileName).Select(vector => vector.Name).ToArray();

	// One artifact file into a Name-keyed dictionary, each row joined to its input by Name. Keying by Name
	// rejects duplicates. Whether the names line up with input.json is the integrity test's job; if one still
	// slips through, the throw here names it instead of surfacing a raw KeyNotFoundException.
	public static Dictionary<string, ArtifactCase> Load(string fileName, ArtifactCodec codec)
	{
		var artifacts = new Dictionary<string, ArtifactCase>();
		foreach (var vector in VectorReader.Read<ArtifactVector>(fileName))
		{
			if (!inputs.TryGetValue(vector.Name, out var input))
				throw new InvalidOperationException(
					$"Artifact '{vector.Name}' in '{fileName}' has no matching input in input.json — " +
					"rerun the generator (the integrity test names the mismatch).");

			// A compressed artifact carries the input's header with the compressed bit set - deflate and gzip
			// are indistinguishable on the wire (§6).
			var header = codec == ArtifactCodec.Raw
				? input.ExpectedHeader
				: input.ExpectedHeader with { Compressed = true };

			var artifact = new ArtifactCase(
				vector.Producer,
				vector.Name,
				codec,
				header,
				Convert.FromBase64String(vector.Base64),
				input);
			artifacts.Add(vector.Name, artifact);
		}

		return artifacts;
	}

	// Raw interop/input.json row: a (bin, items) scenario plus its expected header, in the shared string forms.
	private sealed class InputVector
	{
		public string Name { get; set; } = "";
		public string ExpectedHeader { get; set; } = "";
		public string Bin { get; set; } = "";
		public string[] Items { get; set; } = [];
	}

	// Raw artifact-*.json row: the base64 blob a producer emitted (the expected header is on the input, joined
	// by Name).
	private sealed class ArtifactVector
	{
		public string Name { get; set; } = "";
		public string Producer { get; set; } = "";
		public string Base64 { get; set; } = "";
	}
}
