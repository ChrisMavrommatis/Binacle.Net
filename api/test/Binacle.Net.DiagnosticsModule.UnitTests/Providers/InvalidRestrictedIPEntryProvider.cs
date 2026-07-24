using System.Collections;

namespace Binacle.Net.DiagnosticsModule.UnitTests.Providers;

// Entries that must be refused, both by the parser and by startup validation. The start-end range form is here
// because it used to be supported: v3.0.0 removed it, so an operator carrying one forward has to be told.
// Row: entry.
internal class InvalidRestrictedIPEntryProvider : IEnumerable<object[]>
{
	public IEnumerator<object[]> GetEnumerator()
	{
		yield return ["192.168.1.1-192.168.1.9"]; // the removed range form
		yield return ["192.168.1.0/33"]; // prefix length past the address size
		yield return ["not-an-address"];
		yield return [""];
		yield return ["   "];
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
