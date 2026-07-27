using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace Binacle.Net.Kernel.Network;

// An IP entry as an operator writes it in configuration: a single address, or CIDR notation. Anywhere a module
// reads a list of addresses from a config file - a health check allow-list, a trusted proxy list - it reads them
// through here, so one spelling means one thing across the app.
public static class IPEntry
{
	// "192.168.1.1", or "192.168.1.1/24". The slash picks which, and nothing else is attempted, so a mistyped
	// prefix comes back as a bad prefix rather than as whichever parser happened to fail last.
	public static bool TryParse(string? value, out IPNetwork network)
	{
		network = default;

		if (string.IsNullOrWhiteSpace(value))
		{
			return false;
		}

		// Neither parser tolerates surrounding whitespace, and a padded entry in a JSON list is a typo rather than
		// a different entry. Whitespace-only is already refused above, so it stays invalid.
		var entry = value.Trim();
		var slashIndex = entry.IndexOf('/');
		var addressText = slashIndex < 0 ? entry : entry[..slashIndex];

		// The one guard this type exists for. IPAddress.TryParse still reads the inet_aton forms - "010.10.10.10"
		// is octal and lands on 8.10.10.10, "0x0A.10.10.10" is hex, "10.1" and "167772161" both come out as
		// 10.0.0.1 - and it drops an IPv6 scope id instead of matching on it. An entry that admits a host other
		// than the one it reads as is the whole risk here, so an entry has to come back out of ToString exactly as
		// it went in. IPv6 is held to the same rule, which is why "2001:0db8::1" has to be written "2001:db8::1":
		// one rule beats a table of refused spellings.
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
			// IPAddress.TryParse hands back only those two, but AddressFamily has thirty-odd values. Refused
			// rather than guessed at: a ternary here gives every other family an IPv6-sized prefix, and TryParse
			// is not allowed to throw on something that came out of a config file.
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

		// The prefix is decimal in every parser, so a padded "024" is untidy rather than ambiguous and is let
		// through. A sign or a second slash is not a prefix at all, which NumberStyles.None refuses.
		var prefixText = entry[(slashIndex + 1)..];
		if (!int.TryParse(prefixText, NumberStyles.None, CultureInfo.InvariantCulture, out var prefixLength)
		    || prefixLength > fullPrefixLength)
		{
			return false;
		}

		// Host bits are masked off, the same as everywhere else in .NET: "192.168.1.1/24" is the whole
		// 192.168.1.0/24. Callers that hand this list to an operator should say so - 256 addresses is not what that
		// entry looks like. Note the prefix is checked against the family after normalising, so an IPv4-mapped CIDR
		// entry such as ::ffff:192.168.1.0/120 is refused here rather than parsing and then matching no caller.
		network = new IPNetwork(address, prefixLength);
		return true;
	}

	// A dual mode socket reports an IPv4 caller as an IPv4 mapped IPv6 address, and no IPv4 range would ever match
	// that, so the caller and the entry are both unmapped before they meet.
	public static IPAddress Normalize(IPAddress address)
		=> address.IsIPv4MappedToIPv6 ? address.MapToIPv4() : address;
}
