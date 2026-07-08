using System.Text.Encodings.Web;
using System.Text.Json;

namespace Binacle.ViPaq.VectorGenerators;

// Serializes a list of rows as JSON with one object per line. Used for encoding-info-bytes.json — a 256-row
// combinatorial file that's only readable and greppable one-per-line; the interop artifacts stay expanded
// (WriteIndented). A serializer has no "compact but per-line" mode: WriteIndented spreads each object across
// several lines and the default puts the whole array on one line. So each row is serialized compact
// (WriteIndented = false) and the rows are joined by hand into "[\n\t{...},\n\t{...}\n]\n". The row type still
// owns the schema; this only picks the layout. We don't reproduce the space after the colon — one object per
// line is the only goal. UnsafeRelaxedJsonEscaping keeps any '+'/'/' literal, so the helper is safe to reuse.
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
