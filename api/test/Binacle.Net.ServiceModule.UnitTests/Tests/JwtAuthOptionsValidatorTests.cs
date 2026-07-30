using Binacle.Net.ServiceModule.Configuration;

namespace Binacle.Net.ServiceModule.UnitTests;

// These settings sign and check every token the module issues. A weak or missing one is not a runtime error —
// it is a working deployment with worthless tokens, so startup is the only place to catch it.
[Trait("Behavioral Tests", "Ensures JWT auth configuration is validated as expected")]
public class JwtAuthOptionsValidatorTests
{
	private readonly JwtAuthOptionsValidator validator = new();

	// 70 characters is the floor the validator sets; anything shorter is a signing key not worth having.
	private const string usableSecret = "a-token-secret-long-enough-to-satisfy-the-seventy-character-minimum-x";

	private static JwtAuthOptions OptionsWith(
		string? issuer = "binacle",
		string? audience = "binacle",
		string? tokenSecret = usableSecret + "y",
		int expirationInSeconds = 3600
	) => new()
	{
		Issuer = issuer,
		Audience = audience,
		TokenSecret = tokenSecret,
		ExpirationInSeconds = expirationInSeconds
	};

	[Fact]
	public void A_Complete_Configuration_Is_Valid()
	{
		this.validator.Validate(OptionsWith()).IsValid.ShouldBeTrue();
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	public void A_Missing_Issuer_Fails_Validation(string? issuer)
	{
		this.validator.Validate(OptionsWith(issuer: issuer)).IsValid.ShouldBeFalse();
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	public void A_Missing_Audience_Fails_Validation(string? audience)
	{
		this.validator.Validate(OptionsWith(audience: audience)).IsValid.ShouldBeFalse();
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("too-short")]
	public void A_Missing_Or_Short_Token_Secret_Fails_Validation(string? tokenSecret)
	{
		this.validator.Validate(OptionsWith(tokenSecret: tokenSecret)).IsValid.ShouldBeFalse();
	}

	[Fact]
	public void A_Token_Secret_At_The_Minimum_Length_Is_Accepted()
	{
		usableSecret.Length.ShouldBe(69);

		this.validator.Validate(OptionsWith(tokenSecret: usableSecret)).IsValid.ShouldBeFalse();
		this.validator.Validate(OptionsWith(tokenSecret: usableSecret + "z")).IsValid.ShouldBeTrue();
	}

	// Two minutes is the floor. A shorter expiry would have clients re-authenticating faster than a token is
	// useful, which reads as the login endpoint being broken.
	[Theory]
	[InlineData(0)]
	[InlineData(60)]
	[InlineData(119)]
	public void An_Expiration_Below_The_Floor_Fails_Validation(int expirationInSeconds)
	{
		this.validator.Validate(OptionsWith(expirationInSeconds: expirationInSeconds)).IsValid.ShouldBeFalse();
	}

	[Theory]
	[InlineData(120)]
	[InlineData(3600)]
	public void An_Expiration_At_Or_Above_The_Floor_Is_Valid(int expirationInSeconds)
	{
		this.validator.Validate(OptionsWith(expirationInSeconds: expirationInSeconds)).IsValid.ShouldBeTrue();
	}

	// One error per missing setting. Split rules used to report each empty value twice, so an operator with an
	// empty section got eight lines for four settings and had to work out which were duplicates.
	[Fact]
	public void Each_Missing_Setting_Is_Reported_Once()
	{
		var result = this.validator.Validate(new JwtAuthOptions());

		result.Errors.Count.ShouldBe(4);
		result.Errors.Select(error => error.PropertyName).ShouldBe(
			["Issuer", "Audience", "TokenSecret", "ExpirationInSeconds"],
			ignoreOrder: true
		);
	}

	// A message an operator can act on without opening the source: what the floor is, and what they gave.
	[Fact]
	public void The_Token_Secret_Message_States_The_Minimum_And_What_Was_Given()
	{
		var result = this.validator.Validate(OptionsWith(tokenSecret: "short"));

		var message = result.Errors.Single().ErrorMessage;
		message.ShouldContain("70");
		message.ShouldContain("5");
	}

}
