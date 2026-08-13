using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace Binacle.Net.Kernel.Network;

// An IP entry as an operator writes it in configuration: a single address, or CIDR notation. Every config
// address list reads through here, so one spelling means one thing across the app.
public static class IPEntry
{
	// The slash picks the form and nothing else is attempted, so a mistyped prefix comes back as a bad prefix
	// rather than as whichever parser happened to fail last.
	public static bool TryParse(string? value, out IPNetwork network)
	{
		network = default;

		if (string.IsNullOrWhiteSpace(value))
		{
			return false;
		}

		// Neither parser tolerates surrounding whitespace, and a padded entry in a JSON list is a typo.
		var entry = value.Trim();
		var slashIndex = entry.IndexOf('/');
		var addressText = slashIndex < 0 ? entry : entry[..slashIndex];

		// The one guard this type exists for: an entry must never admit a host other than the one it reads as.
		// IPAddress.TryParse still accepts the inet_aton forms, so an address has to come back out of ToString
		// exactly as it went in. What that refuses:
		//
		//   "010.10.10.10"    octal, lands on 8.10.10.10
		//   "0x0A.10.10.10"   hex, lands on 10.10.10.10
		//   "10.1"            short form, lands on 10.0.0.1
		//   "167772161"       bare integer, the same 10.0.0.1
		//   "2001:0db8::1"    IPv6 held to the same rule, so it must be written "2001:db8::1"
		//
		// A scope id needs its own check: "fe80::1%1" survives ToString intact, so the round-trip alone would let
		// it through, and a scope id is not something a network entry can match on.
		if (!IPAddress.TryParse(addressText, out var address)
		    || addressText.Contains('%')
		    || address.ToString() != addressText)
		{
			return false;
		}

		address = Normalize(address);

		var fullPrefixLength = address.AddressFamily switch
		{
			AddressFamily.InterNetwork => 32,
			AddressFamily.InterNetworkV6 => 128,
			// IPAddress.TryParse hands back only those two, but AddressFamily has thirty-odd values. A ternary
			// here would give every other family an IPv6-sized prefix.
			_ => 0
		};

		if (fullPrefixLength == 0)
		{
			return false;
		}

		if (slashIndex < 0)
		{
			// A single address is a range of one, so it carries the full prefix for its family.
			network = new IPNetwork(address, fullPrefixLength);
			return true;
		}

		// The prefix is decimal in every parser, so a padded "024" is untidy rather than ambiguous. A sign or a
		// second slash is not a prefix at all, which NumberStyles.None refuses.
		var prefixText = entry[(slashIndex + 1)..];
		if (!int.TryParse(prefixText, NumberStyles.None, CultureInfo.InvariantCulture, out var prefixLength)
		    || prefixLength > fullPrefixLength)
		{
			return false;
		}

		// Host bits are masked off, the same as everywhere else in .NET: "192.168.1.1/24" is the whole
		// 192.168.1.0/24, which is 256 addresses. The prefix is checked against the family after normalising, so
		// an IPv4-mapped CIDR entry such as ::ffff:192.168.1.0/120 is refused here rather than parsing and then
		// matching no caller.
		network = new IPNetwork(address, prefixLength);
		return true;
	}

	// A dual mode socket reports an IPv4 caller as an IPv4-mapped IPv6 address, which no IPv4 range matches, so
	// both sides are unmapped first.
	public static IPAddress Normalize(IPAddress address)
		=> address.IsIPv4MappedToIPv6 ? address.MapToIPv4() : address;
}
