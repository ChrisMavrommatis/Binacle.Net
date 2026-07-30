using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Binacle.ViPaq.UnitTests.Providers;

// Reads the shared cross-language vectors in vipaq/test-vectors/. They are embedded into this assembly (see
// the .csproj), so these tests grade against the exact same files the TypeScript suite reads and neither side
// can drift on the wire format. Callers pass the on-disk path ("protocol/little-endian/uint8.json") — the same
// string the TS readVectors takes — so the two readers line up. Edit a case in the JSON, not here.
internal static class VectorReader
{
	private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

	private static readonly JsonSerializerOptions Options = new()
	{
		// JSON keys and POCO properties are both PascalCase, so no case-insensitive matching is needed.
		PropertyNameCaseInsensitive = false,
		ReadCommentHandling = JsonCommentHandling.Skip,
		Converters = { new JsonStringEnumConverter() }, // BitSize etc. arrive as enum names ("Eight").
	};

	// Reads a vector file (e.g. "serialization/exact-bytes.json" or "protocol/little-endian/uint16.json") into its
	// rows. The files are embedded under a flattened "Data.<dotted>" logical name (see the .csproj), so '/' maps
	// to '.' here.
	public static T[] Read<T>(string fileName)
	{
		var resourceName = "Data." + fileName.Replace('/', '.');

		using var stream = Assembly.GetManifestResourceStream(resourceName)
			?? throw new FileNotFoundException(
				$"Embedded vector '{resourceName}' not found. Available: " +
				string.Join(", ", Assembly.GetManifestResourceNames()));

		return JsonSerializer.Deserialize<T[]>(stream, Options)
			?? throw new InvalidOperationException($"Vector '{fileName}' deserialized to null.");
	}
}
