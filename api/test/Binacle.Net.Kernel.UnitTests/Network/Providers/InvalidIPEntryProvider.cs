using System.Collections;

namespace Binacle.Net.Kernel.UnitTests.Network.Providers;

// Entries that must be refused. Most of them parse fine in the BCL and mean a host other than the one they read
// as, which is the whole reason IPEntry exists. Row: entry.
internal class InvalidIPEntryProvider : IEnumerable<object?[]>
{
	public IEnumerator<object?[]> GetEnumerator()
	{
		yield return ["192.168.1.1-192.168.1.9"]; // the range form, never supported here
		yield return ["192.168.1.0/33"]; // prefix length past the address size
		yield return ["not-an-address"];
		yield return [""];
		yield return ["   "]; // whitespace-only is an entry the operator meant to write and did not
		yield return [null]; // a stray comma in a JSON list

		// Forms IPAddress.TryParse still reads, each landing somewhere other than where it looks like it lands.
		yield return ["010.10.10.10"]; // octal: 8.10.10.10
		yield return ["10.010.10.10"]; // octal: 10.8.10.10
		yield return ["0x0A.10.10.10"]; // hex: 10.10.10.10
		yield return ["10.1"]; // shorthand: 10.0.0.1
		yield return ["10"]; // shorthand: 0.0.0.10
		yield return ["167772161"]; // the whole address as one number: 10.0.0.1
		yield return ["010.10.10.0/24"]; // the same octal reading, inside a prefix
		yield return ["10.1/24"]; // the same shorthand, inside a prefix

		// Spellings the BCL rewrites. IPv6 is held to the canonical form so one rule covers every family.
		yield return ["2001:0db8::1"]; // written out, prints as 2001:db8::1
		yield return ["2001:DB8::/32"]; // uppercase, prints lowercase
		yield return ["[::1]"]; // brackets belong in a URL
		yield return ["fe80::1%eth0"]; // a scope id is dropped, not matched on
		yield return ["fe80::1%12"];

		// Unmapping leaves an IPv4 address and /120 is past /32, so it is refused rather than parsing and then
		// matching no caller.
		yield return ["::ffff:192.168.1.0/120"];

		yield return ["192.168.1.0/"];
		yield return ["/24"];
		yield return ["192.168.1.0/24/8"];
		yield return ["192.168.1.0/-1"];
		yield return ["192.168.1.0/ 24"];
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
