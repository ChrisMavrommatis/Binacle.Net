using System.Text.Encodings.Web;
using System.Text.Json;
using Binacle.CompactNotation;
using Binacle.Geometry;
using Binacle.TestReporting;
using Binacle.ViPaq;
using Binacle.ViPaq.Compression;

namespace Binacle.ViPaq.VectorGenerators;

// Encodes each shared interop input with the C# ViPaq library and writes the bytes (base64) to the interop/cs
// folder, one file per codec: cs/raw.json, cs/deflate.json, cs/gzip.json. The TS tool mirrors this into interop/ts
// off the same input.
//
// It drives ProtocolEncoder, not ViPaqSerializer, so it obeys each scenario's ExpectedHeader — that is what lets
// it emit the columnar and wider scenarios ViPaqSerializer's narrowest-raw choice would not, and force
// compression on. Raw uses the NoOp codec; deflate/gzip set the header's compressed bit and run the real codec.
// The point of the matrix is the cross-language decode test: each language must read all three, from either
// producer (PROTOCOL.md §6.1 — decode-to-input, never byte comparison for the compressed ones).
public sealed class InteropArtifactGenerator : IVectorGenerator
{
	public void Generate()
	{
		var interopDir = RepositoryRoot.Bind().Find("vipaq", "test-vectors", "interop");
		var inputPath = Path.Combine(interopDir, "input.json");

		// This is the C# producer, so it only ever writes its own folder — no language stem anywhere.
		var outputDir = Path.Combine(interopDir, "cs");
		Directory.CreateDirectory(outputDir);

		var readOptions = new JsonSerializerOptions
		{
			// Keys and properties are both PascalCase, so no case-insensitive matching is needed.
			PropertyNameCaseInsensitive = false,
			ReadCommentHandling = JsonCommentHandling.Skip,
		};

		var inputs = JsonSerializer.Deserialize<InputScenario[]>(File.ReadAllText(inputPath), readOptions)
		             ?? throw new InvalidOperationException("input.json deserialized to null.");

		var writeOptions = new JsonSerializerOptions
		{
			WriteIndented = true,
			IndentCharacter = '\t',
			IndentSize = 1,
			// Keep base64 '+' and '/' literal instead of \u escapes — the content is plain base64, so this
			// is safe and the committed file stays readable.
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
		};

		(string Suffix, ICompressionCodec Codec)[] modes =
		[
			("raw", new NoOpCodec()),
			("deflate", new DeflateCodec()),
			("gzip", new GzipCodec()),
		];

		foreach (var mode in modes)
		{
			var encoder = new ProtocolEncoder(mode.Codec);
			var artifacts = new List<Artifact>();
			foreach (var input in inputs)
			{
				// Geometry via the shared notation; the bin is dimensions-only, items are the shared placed model.
				var bin = CompactNotationParser.ParseDimensions<long>(input.Bin);
				var items = CompactNotationParser.ParseItems<long>(input.Items).ToList();

				// Encode as long — long holds the whole interoperable range [0, 65_535] exactly. The header comes
				// from the scenario; a compressed mode reuses it with the compressed bit set. The bit tracks the
				// codec: raw uses NoOp and stays uncompressed, deflate/gzip set the bit.
				var header = HeaderNotation.Parse(input.ExpectedHeader);
				header = header with { Compressed = mode.Codec is not NoOpCodec };

				var bytes = encoder.Encode<Dimensions<long>, Item<long>, long>(header, bin, items);

				artifacts.Add(new Artifact
				{
					Name = input.Name,
					Producer = "csharp",
					Base64 = Convert.ToBase64String(bytes),
				});
			}

			var outputPath = Path.Combine(outputDir, $"{mode.Suffix}.json");
			File.WriteAllText(outputPath, JsonSerializer.Serialize(artifacts, writeOptions));
			Console.WriteLine($"Wrote {artifacts.Count} {mode.Suffix} artifact(s) to {outputPath}");
		}
	}
}
