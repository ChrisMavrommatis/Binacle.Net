
namespace Binacle.ViPaq.UnitTests.Providers;

// Shared loader for the interop vectors. The file-read + parse code lives here once; CSharpArtifacts and
// TypeScriptArtifacts are thin providers that call Load with their own artifact file. (xUnit [MemberData]
// sources must be static and C# static classes can't derive, so "abstract base + derived providers" is a
// shared static helper + two static providers rather than real inheritance.)
//
// input.json holds the shared scenarios; every artifact blob must deserialize back to the input it names.
internal static class InteropVectors
{
	// ExpectedEncodingInfo lives on the input (the scenario), not the artifact — it's producer-independent and
	// spec-determined, so declaring it here makes the byte-0 pin a real oracle instead of a value echoed back
	// from the generator's own output.
	public sealed record Input(EncodingInfo ExpectedEncodingInfo, Bin<long> Bin, Item<long>[] Items);

	// Producer ("csharp"/"typescript") is kept even though the file name / provider already implies it —
	// it may be needed later (e.g. richer test labels or per-producer assertions).
	public sealed record ArtifactCase(string Producer, string Name, EncodingInfo ExpectedEncodingInfo, byte[] Bytes, Input Input);

	// input.json, loaded once and shared by both providers.
	private static readonly Dictionary<string, Input> inputs;
	static InteropVectors()
	{
		inputs = new Dictionary<string, Input>();
		foreach (var vector in VectorReader.Read<InputVector>(InteropFiles.Input))
		{
			var input = new Input(
				VectorParser.ParseEncodingInfo(vector.ExpectedEncodingInfo),
				VectorParser.ParseBin(vector.Bin),
				VectorParser.ParseItems(vector.Items).ToArray());
			inputs.Add(vector.Name, input);
		}
	}

	public static IReadOnlyCollection<string> InputNames
		=> inputs.Keys;

	// Just the Name of each row in an artifact file, without joining to input.json. The integrity test
	// uses this so it can compare the two Name sets and report the difference itself — reading through
	// Load instead would throw inside a provider's static ctor first, turning a clear "which names differ"
	// into a murky TypeInitializationException.
	public static IReadOnlyList<string> ReadNames(string fileName)
		=> VectorReader.Read<ArtifactVector>(fileName).Select(vector => vector.Name).ToArray();

	// Reads one artifact file into a Name-keyed dictionary, joining each row to its input by Name. Keying
	// by Name rejects duplicate names in the file. Whether the names line up with input.json is the
	// integrity test's job — it runs first (via ReadNames) and fails clearly; if a name still slips
	// through to here the throw names it plainly instead of surfacing a raw KeyNotFoundException.
	public static Dictionary<string, ArtifactCase> Load(string fileName)
	{
		var artifacts = new Dictionary<string, ArtifactCase>();
		foreach (var vector in VectorReader.Read<ArtifactVector>(fileName))
		{
			if (!inputs.TryGetValue(vector.Name, out var input))
				throw new InvalidOperationException(
					$"Artifact '{vector.Name}' in '{fileName}' has no matching input in input.json — " +
					"rerun the generator (the integrity test names the mismatch).");

			var artifact = new ArtifactCase(
				vector.Producer,
				vector.Name,
				input.ExpectedEncodingInfo,
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
		public string ExpectedEncodingInfo { get; set; } = "";
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
