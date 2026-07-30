using System.Net;
using Binacle.Net.Kernel.Network;
using Binacle.Net.Kernel.UnitTests.Network.Providers;

namespace Binacle.Net.Kernel.UnitTests.Network;

// What an entry in a configured IP list means. Modules match callers against these, so a spelling that parses
// to a different host than it reads as is the failure this covers.
[Trait("Behavioral Tests", "Ensures configured IP entries parse and match as expected")]
public class IPEntryTests
{
	[Theory]
	[ClassData(typeof(ValidIPEntryProvider))]
	public void TryParse_Returns_Expected_Network_For_Entry(
		string entry,
		string expectedBaseAddress,
		int expectedPrefixLength
	)
	{
		var parsed = IPEntry.TryParse(entry, out var network);

		parsed.ShouldBeTrue();
		network.BaseAddress.ShouldBe(IPAddress.Parse(expectedBaseAddress));
		network.PrefixLength.ShouldBe(expectedPrefixLength);
	}

	[Theory]
	[ClassData(typeof(InvalidIPEntryProvider))]
	public void TryParse_Rejects_An_Unsupported_Entry(string? entry)
	{
		IPEntry.TryParse(entry, out _).ShouldBeFalse();
	}

	// Each of these pins a decision against the BCL's own reading, so the BCL's answer is asserted first. Without
	// that contrast the strictness looks like a bug and gets removed.
	[Fact]
	public void A_Leading_Zero_Is_Refused_Rather_Than_Read_As_Octal()
	{
		IPAddress.TryParse("010.10.10.10", out var lenient).ShouldBeTrue();
		lenient.ShouldBe(IPAddress.Parse("8.10.10.10"));

		IPEntry.TryParse("010.10.10.10", out _).ShouldBeFalse();
	}

	[Fact]
	public void A_Shorthand_Address_Is_Refused_Rather_Than_Filled_In()
	{
		IPAddress.TryParse("10.1", out var lenient).ShouldBeTrue();
		lenient.ShouldBe(IPAddress.Parse("10.0.0.1"));

		IPEntry.TryParse("10.1", out _).ShouldBeFalse();
	}

	// The one place the BCL's leniency is kept, because it is what CIDR notation means everywhere.
	[Fact]
	public void An_Entry_With_Host_Bits_Set_Covers_Its_Whole_Block()
	{
		IPEntry.TryParse("192.168.1.1/24", out var network).ShouldBeTrue();

		network.BaseAddress.ShouldBe(IPAddress.Parse("192.168.1.0"));
		network.Contains(IPAddress.Parse("192.168.1.255")).ShouldBeTrue();
	}

	// The value after the slash used to be read as an address mask elsewhere in the app, so "192.168.1.0/24"
	// admitted nearly the whole IPv4 range. As a prefix length it admits 256 addresses.
	[Fact]
	public void A_Slash_24_Entry_Covers_Its_Own_Block_And_Nothing_Outside_It()
	{
		IPEntry.TryParse("192.168.1.0/24", out var network).ShouldBeTrue();

		network.Contains(IPAddress.Parse("192.168.1.0")).ShouldBeTrue();
		network.Contains(IPAddress.Parse("192.168.1.255")).ShouldBeTrue();
		network.Contains(IPAddress.Parse("192.168.0.255")).ShouldBeFalse();
		network.Contains(IPAddress.Parse("192.168.2.0")).ShouldBeFalse();
	}

	[Fact]
	public void Normalize_Unwraps_An_IPv4_Mapped_Address()
	{
		var normalized = IPEntry.Normalize(IPAddress.Parse("::ffff:192.168.1.5"));

		normalized.ShouldBe(IPAddress.Parse("192.168.1.5"));
	}

	[Theory]
	[InlineData("192.168.1.5")]
	[InlineData("2001:db8::1")]
	public void Normalize_Leaves_An_Unmapped_Address_Alone(string address)
	{
		var normalized = IPEntry.Normalize(IPAddress.Parse(address));

		normalized.ShouldBe(IPAddress.Parse(address));
	}
}
