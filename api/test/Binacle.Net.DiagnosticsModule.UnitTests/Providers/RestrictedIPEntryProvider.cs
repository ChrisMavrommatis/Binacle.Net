using System.Collections;

namespace Binacle.Net.DiagnosticsModule.UnitTests.Providers;

// Allow-list entries an operator can write, and what each one parses to.
// Row: entry, expected base address, expected prefix length. A single address is a network of one, so it
// carries the full prefix for its family — /32 for IPv4, /128 for IPv6.
internal class RestrictedIPEntryProvider : IEnumerable<object[]>
{
	public IEnumerator<object[]> GetEnumerator()
	{
		yield return ["192.168.1.0/24", "192.168.1.0", 24];
		yield return ["10.0.0.0/8", "10.0.0.0", 8];
		yield return ["2001:db8::/32", "2001:db8::", 32];

		yield return ["192.168.1.1", "192.168.1.1", 32];
		yield return ["2001:db8::1", "2001:db8::1", 128];
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
