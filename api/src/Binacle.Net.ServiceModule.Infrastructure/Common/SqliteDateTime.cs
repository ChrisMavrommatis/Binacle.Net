using System.Globalization;

namespace Binacle.Net.ServiceModule.Infrastructure.Common;

// Sqlite has no date type - the Accounts and Subscriptions tables store these columns as TEXT. This is the
// one place that decides what that text looks like, so the read and write sides cannot drift apart.
internal static class SqliteDateTime
{
	// Big-endian, so the text sorts the same way the instant does. That is what makes ORDER BY and a range
	// comparison work on a TEXT column at all.
	private const string Format = "yyyy/MM/dd HH:mm:ss";

	// Always InvariantCulture. In a .NET custom format string "/" and ":" are the date and time separator
	// placeholders, not literals - under a culture that uses "." or "," they come out as that instead, and
	// the same build writes a different format on a different machine.
	public static string ToStorage(DateTimeOffset value)
		=> value.ToUniversalTime().ToString(Format, CultureInfo.InvariantCulture);

	// Parse, not ParseExact: rows written before this format existed are in the invariant "G" form
	// ("08/09/2026 14:30:00 +00:00") and still have to read. AssumeUniversal covers the offset the format
	// deliberately does not carry, since ToStorage has already converted to UTC.
	public static DateTimeOffset FromStorage(string value)
		=> DateTimeOffset.Parse(
			value,
			CultureInfo.InvariantCulture,
			DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
		);
}
