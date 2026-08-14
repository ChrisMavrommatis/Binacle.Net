using System.Text.Encodings.Web;
using System.Text.Json;

namespace Binacle.ViPaq.VectorGenerators;

// Serializes a list of rows as JSON, one object per line, so a large combinatorial file stays greppable.
//
// The serializer has no "compact but per-line" mode: WriteIndented spreads each object over several lines and
// the default puts the whole array on one. So each row is serialized compact and the rows are joined by hand.
// UnsafeRelaxedJsonEscaping keeps any '+' or '/' literal.
public static class CompactJson
{
	private static readonly JsonSerializerOptions RowOptions = new()
	{
		WriteIndented = false,
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
	};

	public static string SerializeArray<T>(IEnumerable<T> rows)
	{
		var lines = rows.Select(row => "\t" + JsonSerializer.Serialize(row, RowOptions));
		return "[\n" + string.Join(",\n", lines) + "\n]\n";
	}
}
