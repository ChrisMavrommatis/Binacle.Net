using System.Collections;

namespace Binacle.Net.DiagnosticsModule.UnitTests.Providers;

// Who a configured allow-list lets through to the health check. Row: allow-list entry, caller address, allowed.
// The IPv4-mapped caller is the container case — a dual mode socket reports an IPv4 caller as ::ffff:a.b.c.d,
// and before v3.0.0 no IPv4 entry could match it.
internal class HealthCheckCallerProvider : IEnumerable<object[]>
{
	public IEnumerator<object[]> GetEnumerator()
	{
		yield return ["192.168.1.0/24", "192.168.1.0", true];
		yield return ["192.168.1.0/24", "192.168.1.5", true];
		yield return ["192.168.1.0/24", "192.168.1.255", true];
		yield return ["192.168.1.0/24", "::ffff:192.168.1.5", true];

		// Neighbours the old mask reading admitted. These are the callers an upgrade shuts out.
		yield return ["192.168.1.0/24", "192.168.0.255", false];
		yield return ["192.168.1.0/24", "192.168.2.0", false];
		yield return ["192.168.1.0/24", "10.0.0.1", false];

		yield return ["192.168.1.1", "192.168.1.1", true];
		yield return ["192.168.1.1", "192.168.1.2", false];
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
