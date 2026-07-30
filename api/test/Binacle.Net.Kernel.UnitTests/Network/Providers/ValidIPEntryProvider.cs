using System.Collections;

namespace Binacle.Net.Kernel.UnitTests.Network.Providers;

// Entries an operator can write, and what each one parses to.
// Row: entry, expected base address, expected prefix length. A single address is a network of one, so it carries
// the full prefix for its family - /32 for IPv4, /128 for IPv6.
internal class ValidIPEntryProvider : IEnumerable<object[]>
{
	public IEnumerator<object[]> GetEnumerator()
	{
		yield return ["192.168.1.0/24", "192.168.1.0", 24];
		yield return ["10.0.0.0/8", "10.0.0.0", 8];
		yield return ["2001:db8::/32", "2001:db8::", 32];

		yield return ["192.168.1.1", "192.168.1.1", 32];
		yield return ["2001:db8::1", "2001:db8::1", 128];

		// Padding around an entry is a typo, not a different entry - both parsers reject it without the trim.
		yield return ["  10.0.0.1  ", "10.0.0.1", 32];
		yield return [" 192.168.1.0/24 ", "192.168.1.0", 24];

		// Host bits are masked off, as they are everywhere else in .NET. The entry names one host and admits 256,
		// so the caller warns about it, but it parses.
		yield return ["192.168.1.1/24", "192.168.1.0", 24];
		yield return ["192.168.1.255/24", "192.168.1.0", 24];

		// The prefix is decimal in every parser, so a padded one is untidy rather than ambiguous.
		yield return ["192.168.1.0/024", "192.168.1.0", 24];

		// A mapped single address is unmapped and matched as IPv4. Its CIDR form is not - see InvalidIPEntryProvider.
		yield return ["::ffff:192.168.1.5", "192.168.1.5", 32];
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
