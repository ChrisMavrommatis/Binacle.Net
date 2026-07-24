using System.Net;

namespace Binacle.Net.DiagnosticsModule.Models;

internal static class RestrictedIPNetwork
{
	public static bool TryParse(string? value, out IPNetwork network)
	{
		network = default;

		if (string.IsNullOrWhiteSpace(value))
		{
			return false;
		}

		if (IPNetwork.TryParse(value, out network))
		{
			return true;
		}

		if (!IPAddress.TryParse(value, out var address))
		{
			return false;
		}

		// A single address is a range of one. Normalising it here leaves the caller with a single kind of entry to
		// match against instead of two.
		address = Normalize(address);
		network = new IPNetwork(address, address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? 32 : 128);
		return true;
	}

	// A dual mode socket reports an IPv4 caller as an IPv4 mapped IPv6 address, and no IPv4 range will ever match
	// that. Both the caller and the configured entry go through here so the two are always compared in the same
	// address family.
	public static IPAddress Normalize(IPAddress address)
		=> address.IsIPv4MappedToIPv6 ? address.MapToIPv4() : address;
}
