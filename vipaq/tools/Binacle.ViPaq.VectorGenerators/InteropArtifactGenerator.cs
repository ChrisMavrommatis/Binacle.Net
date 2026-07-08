using System.Text.Encodings.Web;
using System.Text.Json;
using Binacle.CompactNotation;
using Binacle.Geometry;
using Binacle.TestReporting;

namespace Binacle.ViPaq.VectorGenerators;

// Serializes each shared interop input with the C# ViPaq library and writes the bytes (base64) to
// artifact-cs.json. The TS tool mirrors this, writing the same shape to artifact-ts.json off the same input.
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

			// Serialize as long — long holds the whole interoperable range [0, 2^53 - 1] exactly, and the
			// section width is chosen from the value, not the type, so small values still emit 8-bit sections.
			var bytes = ViPaqSerializer.Serialize<Dimensions<long>, Item<long>, long>(bin, items);

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
