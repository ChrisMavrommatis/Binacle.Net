using Binacle.Net.Configuration;

namespace Binacle.Net.UnitTests;

// Forwarded headers decide who the caller is, which is what rate limiting partitions on and what the health
// check allow-list matches. A configuration that trusts the wrong thing does not fail visibly - it starts
// believing a header anyone can write - so these rules have to hold at startup.
[Trait("Behavioral Tests", "Ensures forwarded headers configuration is validated as expected")]
public class ForwardedHeadersConfigurationOptionsValidatorTests
{
	private readonly ForwardedHeadersConfigurationOptionsValidator validator = new();

	// The element is nullable because a null entry is one of the cases under test - binding gives TrustedProxies a
	// null element, which is what a stray comma in the JSON file produces.
	private static ForwardedHeadersConfigurationOptions OptionsWith(
		bool trustLoopback = true,
		bool trustPrivateNetworks = true,
		string?[]? trustedProxies = null,
		int forwardLimit = 1
	) => new()
	{
		Enabled = true,
		TrustLoopback = trustLoopback,
		TrustPrivateNetworks = trustPrivateNetworks,
		TrustedProxies = trustedProxies!,
		ForwardLimit = forwardLimit
	};

	// The framework treats "nothing trusted" as "check nothing", so it believes every caller's header rather
	// than matching none. Refusing to start is the only safe answer.
	[Fact]
	public void Enabled_With_Nothing_Trusted_Fails_Validation()
	{
		var options = OptionsWith(trustLoopback: false, trustPrivateNetworks: false, trustedProxies: null);

		this.validator.Validate(options).IsValid.ShouldBeFalse();
	}

	[Fact]
	public void Enabled_With_An_Empty_Trusted_Proxy_List_Fails_Validation()
	{
		var options = OptionsWith(trustLoopback: false, trustPrivateNetworks: false, trustedProxies: []);

		this.validator.Validate(options).IsValid.ShouldBeFalse();
	}

	// Any one of the three trust sources is enough: naming a proxy explicitly should not also require the broad
	// flags.
	[Theory]
	[InlineData(true, false, null)]
	[InlineData(false, true, null)]
	[InlineData(false, false, "172.17.0.1")]
	public void Enabled_With_Any_Trusted_Source_Is_Valid(
		bool trustLoopback,
		bool trustPrivateNetworks,
		string? trustedProxy
	)
	{
		var options = OptionsWith(
			trustLoopback: trustLoopback,
			trustPrivateNetworks: trustPrivateNetworks,
			trustedProxies: trustedProxy is null ? null : [trustedProxy]
		);

		this.validator.Validate(options).IsValid.ShouldBeTrue();
	}

	// Disabled is the default, and nothing inside the When gate applies, so switching the feature off does not
	// require keeping a valid trust list.
	[Fact]
	public void A_Disabled_Configuration_Needs_No_Trusted_Source()
	{
		var options = new ForwardedHeadersConfigurationOptions
		{
			Enabled = false,
			TrustLoopback = false,
			TrustPrivateNetworks = false
		};

		this.validator.Validate(options).IsValid.ShouldBeTrue();
	}

	[Theory]
	[InlineData("172.16.0.0/12")]
	[InlineData("172.17.0.1")]
	[InlineData("2001:db8::/32")]
	[InlineData("2001:db8::1")]
	public void A_Trusted_Proxy_Can_Be_A_Range_Or_A_Single_Address(string trustedProxy)
	{
		var options = OptionsWith(trustedProxies: [trustedProxy]);

		this.validator.Validate(options).IsValid.ShouldBeTrue();
	}

	// Entries are read through IPEntry, so a spelling that parses to a different host is refused rather than
	// trusted. The exhaustive table lives with the parser; these rows prove this list is held to it.
	[Theory]
	[InlineData("not-an-address")]
	[InlineData("172.16.0.1-172.16.0.9")]
	[InlineData("")]
	[InlineData(null)]
	[InlineData("010.10.10.10")] // octal, trusts 8.10.10.10
	[InlineData("172.17.1")] // shorthand, trusts 172.17.0.1
	[InlineData("2886729729")] // the whole address as one number
	[InlineData("2001:0db8::1")] // written out, must be 2001:db8::1
	public void An_Unusable_Trusted_Proxy_Fails_Validation(string? trustedProxy)
	{
		var options = OptionsWith(trustedProxies: [trustedProxy]);

		this.validator.Validate(options).IsValid.ShouldBeFalse();
	}

	// Zero would mean "read no hops", which reads the header and then ignores all of it.
	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public void A_Forward_Limit_Below_One_Fails_Validation(int forwardLimit)
	{
		this.validator.Validate(OptionsWith(forwardLimit: forwardLimit)).IsValid.ShouldBeFalse();
	}

	[Theory]
	[InlineData(1)]
	[InlineData(2)]
	public void A_Forward_Limit_Of_At_Least_One_Is_Valid(int forwardLimit)
	{
		this.validator.Validate(OptionsWith(forwardLimit: forwardLimit)).IsValid.ShouldBeTrue();
	}

	// A whole-object rule reports with an empty property name unless it is named, leaving the operator a
	// sentence with no clue which section produced it.
	[Fact]
	public void The_Nothing_Trusted_Error_Names_The_Section()
	{
		var result = this.validator.Validate(
			OptionsWith(trustLoopback: false, trustPrivateNetworks: false)
		);

		result.Errors.Single().PropertyName.ShouldBe(ForwardedHeadersConfigurationOptions.SectionName);
	}

	[Fact]
	public void An_Unusable_Trusted_Proxy_Message_Names_The_Entry()
	{
		var result = this.validator.Validate(OptionsWith(trustedProxies: ["172.16.0.1-172.16.0.9"]));

		result.Errors.Single().ErrorMessage.ShouldContain("172.16.0.1-172.16.0.9");
	}

}
