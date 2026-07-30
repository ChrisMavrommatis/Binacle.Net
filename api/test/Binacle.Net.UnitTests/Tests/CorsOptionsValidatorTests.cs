using Binacle.Net.Configuration;

namespace Binacle.Net.UnitTests;

// A bad origin never fails here at runtime — the app starts, and the browser silently blocks the request in
// someone else's console. Startup is the only place an operator finds out.
[Trait("Behavioral Tests", "Ensures CORS configuration is validated as expected")]
public class CorsOptionsValidatorTests
{
	private readonly CorsOptionsOptionsValidator validator = new();

	private static CorsOptions OptionsWith(params string[]? allowedOrigins)
		=> new() { CoreApi = new CorsPolicyOptions { AllowedOrigins = allowedOrigins } };

	// The section is optional and a closed policy is a valid choice, so absent and empty both stay valid.
	[Fact]
	public void An_Absent_Or_Empty_Section_Is_Valid()
	{
		this.validator.Validate(new CorsOptions()).IsValid.ShouldBeTrue();
		this.validator.Validate(new CorsOptions { CoreApi = new CorsPolicyOptions() }).IsValid.ShouldBeTrue();
		this.validator.Validate(OptionsWith()).IsValid.ShouldBeTrue();
	}

	[Theory]
	[InlineData("https://example.com")]
	[InlineData("http://localhost:5173")]
	[InlineData("https://sub.example.com:8443")]
	[InlineData("*")]
	public void A_Matchable_Origin_Is_Accepted(string origin)
	{
		var options = OptionsWith(origin);
		this.validator.Validate(options).IsValid.ShouldBeTrue();
	}

	// Each of these is something a browser compares against and never matches, so the request is blocked with no
	// hint as to why.
	[Theory]
	[InlineData("https://example.com/")] // trailing slash
	[InlineData("https://example.com/app")] // path
	[InlineData("https://example.com?q=1")] // query
	[InlineData("example.com")] // no scheme
	[InlineData("ftp://example.com")] // not a browser origin
	[InlineData("")]
	[InlineData("   ")]
	public void An_Unmatchable_Origin_Fails_Validation(string origin)
	{
		var options = OptionsWith(origin);
		this.validator.Validate(options).IsValid.ShouldBeFalse();
	}

	[Fact]
	public void An_Unmatchable_Origin_Message_Names_The_Entry_And_Shows_A_Working_One()
	{
		var options = OptionsWith("https://example.com/");
		var result = this.validator.Validate(options);

		var message = result.Errors.Single().ErrorMessage;
		message.ShouldContain("https://example.com/");
		message.ShouldContain("No trailing slash");
	}

	[Fact]
	public void Only_The_Bad_Entry_Is_Reported()
	{
		var options = OptionsWith("https://good.example.com", "https://bad.example.com/");
		var result = this.validator.Validate(options);

		result.Errors.Count.ShouldBe(1);
		result.Errors.Single().ErrorMessage.ShouldContain("bad.example.com");
	}
}
