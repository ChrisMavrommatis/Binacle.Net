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
		RuleFor(x => x.Issuer).NotNull().NotEmpty();
		RuleFor(x => x.Audience).NotNull().NotEmpty();
		RuleFor(x => x.TokenSecret).NotNull().NotEmpty().MinimumLength(70);
		RuleFor(x => x.ExpirationInSeconds).NotNull().NotEmpty().GreaterThanOrEqualTo(120);
	}
}
