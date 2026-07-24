using Binacle.Net.ServiceModule.Configuration;

namespace Binacle.Net.ServiceModule.UnitTests;

// The rate limits are configured as strings, so a typo is only found when the string is parsed. Startup is the
// place for that — the alternative is a module that starts and then throws on the first request it throttles.
[Trait("Behavioral Tests", "Ensures rate limiter configuration is validated as expected")]
public class RateLimiterConfigurationOptionsValidatorTests
{
	private readonly RateLimiterConfigurationOptionsValidator validator = new();

	private static RateLimiterConfigurationOptions OptionsWith(
		string? apiUsageAnonymous = "FixedWindow::5/60",
		string? authToken = "FixedWindow::5/60",
		string? apiUsageDemoSubscription = "FixedWindow::5/60"
	) => new()
	{
		ApiUsageAnonymous = apiUsageAnonymous,
		AuthToken = authToken,
		ApiUsageDemoSubscription = apiUsageDemoSubscription
	};

	[Theory]
	[InlineData("FixedWindow::5/60")]
	[InlineData("SlidingWindow::5/60-4")] // PermitLimit/WindowInSeconds-SegmentsPerWindow
	[InlineData("NoLimiter::0")]
	public void A_Parsable_Configuration_Is_Valid(string configuration)
	{
		this.validator.Validate(OptionsWith(authToken: configuration)).IsValid.ShouldBeTrue();
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData("FixedWindow")] // no options section
	[InlineData("FixedWindow::5")] // FixedWindow needs PermitLimit/WindowInSeconds
	[InlineData("FixedWindow::5/60/4")] // too many options
	[InlineData("SlidingWindow::5/60")] // sliding window needs the segment count
	[InlineData("Unknown::5/60")] // not a limiter type
	[InlineData("fixedwindow::5/60")] // the type match is case sensitive
    public void An_Unusable_Configuration_Fails_Validation(string? configuration)
	{
		this.validator.Validate(OptionsWith(authToken: configuration)).IsValid.ShouldBeFalse();
	}

	// Each of the three settings is checked separately, so a broken one is named rather than the section as a whole.
	[Fact]
	public void Each_Limiter_Setting_Is_Validated_On_Its_Own()
	{
		this.validator.Validate(OptionsWith(apiUsageAnonymous: "nonsense")).IsValid.ShouldBeFalse();
		this.validator.Validate(OptionsWith(apiUsageDemoSubscription: "nonsense")).IsValid.ShouldBeFalse();
		this.validator.Validate(OptionsWith(authToken: "nonsense")).IsValid.ShouldBeFalse();
	}

	[Fact]
	public void Each_Missing_Limiter_Is_Reported_Once()
	{
		var result = this.validator.Validate(new RateLimiterConfigurationOptions());

		result.Errors.Count.ShouldBe(3);
	}

	// The limiters are strings with no schema behind them, so the message is the only place the format is
	// written down where an operator will see it.
	[Fact]
	public void An_Unusable_Limiter_Message_Shows_The_Formats_And_The_Value()
	{
		var result = this.validator.Validate(OptionsWith(authToken: "nonsense"));

		var message = result.Errors.Single().ErrorMessage;
		message.ShouldContain("FixedWindow::5/60");
		message.ShouldContain("SlidingWindow::5/60-4");
		message.ShouldContain("NoLimiter::0");
		message.ShouldContain("nonsense");
	}

}
