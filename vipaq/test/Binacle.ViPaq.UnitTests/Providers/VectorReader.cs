using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Binacle.ViPaq.UnitTests.Providers;

// Reads the shared cross-language vectors in vipaq/test-vectors/. They are embedded into this
// assembly (see the .csproj), so these tests grade against the exact same files the TypeScript suite
// reads and neither side can drift on the wire format. Edit a case in the JSON, not here.
internal static class VectorReader
{
	private const string Prefix = "Data.";

	private static readonly JsonSerializerOptions Options = new()
	{
		// JSON keys and POCO properties are both PascalCase, so no case-insensitive matching is needed.
		PropertyNameCaseInsensitive = false,
		ReadCommentHandling = JsonCommentHandling.Skip,
		Converters = { new JsonStringEnumConverter() }, // BitSize etc. arrive as enum names ("Eight").
	};

	// Reads a vector file (e.g. "exact-bytes.json" or "little-endian.uint16.json") into its rows.
	public static T[] Read<T>(string fileName)
	{
		var resourceName = Prefix + fileName;
		var assembly = Assembly.GetExecutingAssembly();

		using var stream = assembly.GetManifestResourceStream(resourceName)
			?? throw new FileNotFoundException(
				$"Embedded vector '{resourceName}' not found. Available: " +
				string.Join(", ", assembly.GetManifestResourceNames()));

		return JsonSerializer.Deserialize<T[]>(stream, Options)
			?? throw new InvalidOperationException($"Vector '{fileName}' deserialized to null.");
	}
}
