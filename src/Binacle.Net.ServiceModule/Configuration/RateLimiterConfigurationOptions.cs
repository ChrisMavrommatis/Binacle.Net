using Binacle.Net.Kernel.Configuration.Models;
using Binacle.Net.ServiceModule.Helpers;
using FluentValidation;

namespace Binacle.Net.ServiceModule.Configuration;

internal class RateLimiterConfigurationOptions : IConfigurationOptions
{
	public static string FilePath => "ServiceModule/RateLimiter.json";
	public static string SectionName => "RateLimiter";
	public static bool Optional => false;
	public static bool ReloadOnChange => false;
	public static string GetEnvironmentFilePath(string environment) => $"ServiceModule/RateLimiter.{environment}.json";

	public string? ApiUsageAnonymous { get; set; }
	public string? AuthToken { get; set; }
}

internal class RateLimiterConfigurationOptionsValidator : AbstractValidator<RateLimiterConfigurationOptions>
{
	public RateLimiterConfigurationOptionsValidator()
	{
		RuleFor(x => x.ApiUsageAnonymous).NotNull().NotEmpty();

		RuleFor(x => x.ApiUsageAnonymous)
			.Must((value) => RateLimiterConfigurationParser.TryParse(value, out var _))
			.WithMessage("Invalid configuration for ApiUsageAnonymous rate limiter. Please check the configuration."); 
		
		RuleFor(x => x.AuthToken).NotNull().NotEmpty();

		RuleFor(x => x.AuthToken)
			.Must((value) => RateLimiterConfigurationParser.TryParse(value, out var _))
			.WithMessage("Invalid configuration for AuthToken rate limiter. Please check the configuration."); 
	}
}
