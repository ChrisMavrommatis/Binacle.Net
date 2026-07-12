using System.Text.Encodings.Web;
using System.Text.Json;
using Binacle.CompactNotation;
using Binacle.Geometry;
using Binacle.TestReporting;
using Binacle.ViPaq;
using Binacle.ViPaq.Compression;

namespace Binacle.ViPaq.VectorGenerators;

// Encodes each shared interop input with the C# ViPaq library and writes the bytes (base64) to artifact-cs.json.
// The TS tool mirrors this, writing the same shape to artifact-ts.json off the same input.
//
// It drives ProtocolEncoder, not ViPaqSerializer, so it obeys each scenario's ExpectedHeader — that is what lets
// it emit the columnar and wider scenarios ViPaqSerializer's narrowest-raw choice would not. Every scenario is
// uncompressed for now (compression is deferred, PROTOCOL.md §6), so the encoder always gets the NoOp codec, and
// an uncompressed blob is byte-identical to the TS producer's, which the byte-identity test checks.
public sealed class InteropArtifactGenerator : IVectorGenerator
{

	public void Generate()
	{
		var interopDir = RepositoryRoot.Bind().Find("vipaq", "test-vectors", "interop");
		var inputPath = Path.Combine(interopDir, "input.json");
		var outputPath = Path.Combine(interopDir, "artifact-cs.json");

		var readOptions = new JsonSerializerOptions
		{
			// Keys and properties are both PascalCase, so no case-insensitive matching is needed.
			PropertyNameCaseInsensitive = false,
			ReadCommentHandling = JsonCommentHandling.Skip,
		};

		var inputs = JsonSerializer.Deserialize<InputScenario[]>(File.ReadAllText(inputPath), readOptions)
			?? throw new InvalidOperationException("input.json deserialized to null.");

		var artifacts = new List<Artifact>();
		foreach (var input in inputs)
		{
			// Geometry via the shared notation; the bin is dimensions-only, items are the shared placed model.
			var bin = CompactNotationParser.ParseDimensions<long>(input.Bin);
			var items = CompactNotationParser.ParseItems<long>(input.Items).ToList();

			// Encode as long — long holds the whole interoperable range [0, 65_535] exactly. The header comes
			// from the scenario, so the widths, layout and compression are the scenario's choice, not the
			// library's; ProtocolEncoder obeys it.
			var header = HeaderNotation.Parse(input.ExpectedHeader);
			var encoder = new ProtocolEncoder(new NoOpCodec());
			var bytes = encoder.Encode<Dimensions<long>, Item<long>, long>(header, bin, items);

			artifacts.Add(new Artifact
			{
				Name = input.Name,
				Producer = "csharp",
				Base64 = Convert.ToBase64String(bytes),
			});
		}

		var writeOptions = new JsonSerializerOptions
		{
			WriteIndented = true,
			IndentCharacter = '\t',
			IndentSize = 1,
			// Keep base64 '+' and '/' literal instead of \u escapes — the content is plain base64, so this
			// is safe and the committed file stays readable.
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
		};

		File.WriteAllText(outputPath, JsonSerializer.Serialize(artifacts, writeOptions));

		Console.WriteLine($"Wrote {artifacts.Count} artifact(s) to {outputPath}");
		foreach (var artifact in artifacts)
		{
			Console.WriteLine($"  {artifact.Name} ({artifact.Base64.Length} base64 chars)");
		}
	}
}
