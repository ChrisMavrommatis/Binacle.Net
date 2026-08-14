using System.Globalization;

namespace Binacle.Net.ServiceModule.Infrastructure.Common;

// Sqlite has no date type, so these columns are TEXT. One place decides what that text looks like, so the read
// and write sides cannot drift.
internal static class SqliteDateTime
{
	// Big-endian, so the text sorts the same way the instant does. Without that, ORDER BY and a range comparison
	// on a TEXT column do not work.
	private const string Format = "yyyy/MM/dd HH:mm:ss";

	// Always InvariantCulture. In a .NET custom format string "/" and ":" are separator placeholders, not
	// literals, so under a culture using "." or "," the same build writes a different format.
	public static string ToStorage(DateTimeOffset value)
		=> value.ToUniversalTime().ToString(Format, CultureInfo.InvariantCulture);

	// Parse, not ParseExact: rows written before this format existed are in the invariant "G" form
	// ("08/09/2026 14:30:00 +00:00") and still have to read. AssumeUniversal covers the offset the format does
	// not carry, since ToStorage has already converted to UTC.
	public static DateTimeOffset FromStorage(string value)
		=> DateTimeOffset.Parse(
			value,
			CultureInfo.InvariantCulture,
			DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
		);
}
