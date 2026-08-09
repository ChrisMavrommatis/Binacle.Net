using Binacle.Net.Kernel.Configuration.Models;
using FluentValidation;

namespace Binacle.Net.ServiceModule.Configuration;

public class JwtAuthOptions : IConfigurationOptions
{
	public static string FilePath => "ServiceModule/JwtAuth.json";
	public static string SectionName => "JwtAuth";
	public static bool Optional => true;
	public static bool ReloadOnChange => false;
	public static string GetEnvironmentFilePath(string environment) => $"ServiceModule/JwtAuth.{environment}.json";

	public string? Issuer { get; set; }
	public string? Audience { get; set; }
	public string? TokenSecret { get; set; }

	public int ExpirationInSeconds { get; set; }
}


internal class JwtAuthOptionsValidator: AbstractValidator<JwtAuthOptions>
{
	public JwtAuthOptionsValidator()
	{
		// Cascade(Stop) so a missing setting is reported once. Without it NotNull and NotEmpty both fire and share
		// a message, so every empty value was listed twice.
		RuleFor(x => x.Issuer)
			.Cascade(CascadeMode.Stop)
			.NotEmpty();
		RuleFor(x => x.Audience)
			.Cascade(CascadeMode.Stop)
			.NotEmpty();
		RuleFor(x => x.TokenSecret)
			.Cascade(CascadeMode.Stop)
			.NotEmpty()
			.MinimumLength(70)
			.WithMessage(
				"'{PropertyName}' must be at least {MinLength} characters. It signs every token the module "
				+ "issues. You entered {TotalLength}."
			);
		// A non-nullable int, so NotNull could never fail and NotEmpty only meant "not 0" — which then reported
		// a second time as the real rule below. The floor is the only rule there is.
		RuleFor(x => x.ExpirationInSeconds)
			.GreaterThanOrEqualTo(120)
			.WithMessage("'{PropertyName}' must be at least {ComparisonValue} seconds. You entered {PropertyValue}.");
	}
}
