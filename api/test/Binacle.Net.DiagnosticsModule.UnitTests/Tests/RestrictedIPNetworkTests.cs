using System.Net;
using Binacle.Net.DiagnosticsModule.Models;
using Binacle.Net.DiagnosticsModule.UnitTests.Providers;

namespace Binacle.Net.DiagnosticsModule.UnitTests;

// What an allow-list entry means. v3.0.0 changed the reading of the value after the slash, so these cases are
// the contract between what an operator writes in RestrictedIPs and who it admits.
[Trait("Behavioral Tests", "Ensures allow-list entries parse and match as expected")]
public class RestrictedIPNetworkTests
{
	[Theory]
	[ClassData(typeof(RestrictedIPEntryProvider))]
	public void TryParse_Returns_Expected_Network_For_Entry(
		string entry,
		string expectedBaseAddress,
		int expectedPrefixLength
	)
	{
		var parsed = RestrictedIPNetwork.TryParse(entry, out var network);

		parsed.ShouldBeTrue();
		network.BaseAddress.ShouldBe(IPAddress.Parse(expectedBaseAddress));
		network.PrefixLength.ShouldBe(expectedPrefixLength);
	}

	[Theory]
	[ClassData(typeof(InvalidRestrictedIPEntryProvider))]
	public void TryParse_Rejects_An_Unsupported_Entry(string entry)
	{
		RestrictedIPNetwork.TryParse(entry, out _).ShouldBeFalse();
	}

	[Fact]
	public void TryParse_Rejects_A_Null_Entry()
	{
		RestrictedIPNetwork.TryParse(null, out _).ShouldBeFalse();
	}

	// The breaking change, pinned. The value after the slash used to be read as an address mask, so
	// "192.168.1.0/24" admitted nearly the whole IPv4 range. As a prefix length it admits 256 addresses, which
	// means an existing allow-list now lets in fewer callers than it did.
	[Fact]
	public void A_Slash_24_Entry_Covers_Its_Own_Block_And_Nothing_Outside_It()
	{
		RestrictedIPNetwork.TryParse("192.168.1.0/24", out var network).ShouldBeTrue();

		network.Contains(IPAddress.Parse("192.168.1.0")).ShouldBeTrue();
		network.Contains(IPAddress.Parse("192.168.1.255")).ShouldBeTrue();
		network.Contains(IPAddress.Parse("192.168.0.255")).ShouldBeFalse();
		network.Contains(IPAddress.Parse("192.168.2.0")).ShouldBeFalse();
	}

	[Fact]
	public void Normalize_Unwraps_An_IPv4_Mapped_Address()
	{
		var normalized = RestrictedIPNetwork.Normalize(IPAddress.Parse("::ffff:192.168.1.5"));

		normalized.ShouldBe(IPAddress.Parse("192.168.1.5"));
	}

	[Theory]
	[InlineData("192.168.1.5")]
	[InlineData("2001:db8::1")]
	public void Normalize_Leaves_An_Unmapped_Address_Alone(string address)
	{
		var normalized = RestrictedIPNetwork.Normalize(IPAddress.Parse(address));

		normalized.ShouldBe(IPAddress.Parse(address));
	}
}
