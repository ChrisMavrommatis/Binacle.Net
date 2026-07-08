using System.Reflection;
using System.Text.Json;
// [REVIEW-VIPAQ_TEST]
namespace Binacle.ViPaq.TestsKernel.Samples;

// Reads the frozen placed-result files that Binacle.ViPaq.PackedDataGenerator emits under vipaq/data/packed
// (bischoff-suite/ and custom-problems/) and the .csproj embeds as "PackedData.<family>.<file>". Kept tiny and
// self-contained on purpose — the kernel does not reference the ViPaq UnitTests, so it has its own reader rather
// than borrowing VectorReader. Edit the data by regenerating with the tool, not here.
internal static class PackedDataReader
{
	private const string ResourcePrefix = "PackedData.";

	private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

	private static readonly JsonSerializerOptions Options = new()
	{
		// JSON keys and POCO properties are both PascalCase, so no case-insensitive matching is needed.
		PropertyNameCaseInsensitive = false,
		ReadCommentHandling = JsonCommentHandling.Skip,
	};

	// Every embedded packed-data file, flattened to the rows across all of them, each tagged with the source
	// family it came from (the folder — "bischoff-suite" or "custom-problems"), taken from the resource name
	// "PackedData.<family>.<file>". Order follows the sorted resource names so it is stable.
	public static IEnumerable<(string Family, PackedSampleRecord Record)> ReadAll()
	{
		var resourceNames = Assembly.GetManifestResourceNames()
			.Where(name => name.StartsWith(ResourcePrefix, StringComparison.Ordinal))
			.OrderBy(name => name, StringComparer.Ordinal);

		foreach (var resourceName in resourceNames)
		{
			// "PackedData.bischoff-suite.orlib_thpack1.ffd.json" → family "bischoff-suite" (folder names have no dot).
			var family = resourceName[ResourcePrefix.Length..].Split('.')[0];

			using var stream = Assembly.GetManifestResourceStream(resourceName)
				?? throw new FileNotFoundException($"Embedded packed data '{resourceName}' not found.");

			var records = JsonSerializer.Deserialize<PackedSampleRecord[]>(stream, Options)
				?? throw new InvalidOperationException($"Packed data '{resourceName}' deserialized to null.");

			foreach (var record in records)
			{
				yield return (family, record);
			}
		}
	}

	// One row as stored on disk: compact-notation strings plus the derived width family. No token is stored —
	// it is derivable from the geometry and its compressed bytes vary by runtime.
	internal sealed class PackedSampleRecord
	{
		public required string Name { get; init; }
		public required int WidthBits { get; init; }
		public required string Bin { get; init; }
		public required string[] Items { get; init; }
	}
}
