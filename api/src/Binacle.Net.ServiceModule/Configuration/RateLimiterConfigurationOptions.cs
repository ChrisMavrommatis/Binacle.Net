using Binacle.Net.Kernel.Configuration.Models;
using Binacle.Net.ServiceModule.Models;
using FluentValidation;

namespace Binacle.Net.ServiceModule.Configuration;

internal class RateLimiterConfigurationOptions : IConfigurationOptions
{
	public static string FilePath => "ServiceModule/RateLimiter.json";
	public static string SectionName => "RateLimiter";
	public static bool Optional => false;
	public static bool ReloadOnChange => false;
	public static string GetEnvironmentFilePath(string environment) => $"ServiceModule/RateLimiter.{environment}.json";

	public RateLimiterConfiguration ApiUsageAnonymousConfiguration { get; set; } = null!;
	public RateLimiterConfiguration AuthTokenConfiguration { get; set;} = null!;
	public RateLimiterConfiguration ApiUsageDemoSubscriptionConfiguration { get; set; } = null!;
	public string? ApiUsageAnonymous { get; set; }
	public string? AuthToken { get; set; }
	public string? ApiUsageDemoSubscription { get; set; }
	
}

internal class RateLimiterConfigurationOptionsValidator : AbstractValidator<RateLimiterConfigurationOptions>
{
	public RateLimiterConfigurationOptionsValidator()
	{
		RuleFor(x => x.ApiUsageAnonymous).NotNull().NotEmpty();

		RuleFor(x => x.ApiUsageAnonymous)
			.Must((value) => RateLimiterConfiguration.TryParse(value, out var _))
			.WithMessage($"Invalid configuration for '{nameof(RateLimiterConfigurationOptions.ApiUsageAnonymous)}' rate limiter. Please check the configuration."); 
		
		RuleFor(x => x.AuthToken).NotNull().NotEmpty();

		RuleFor(x => x.AuthToken)
			.Must((value) => RateLimiterConfiguration.TryParse(value, out var _))
			.WithMessage($"Invalid configuration for '{nameof(RateLimiterConfigurationOptions.AuthToken)}' rate limiter. Please check the configuration."); 
		
		RuleFor(x => x.ApiUsageDemoSubscription).NotNull().NotEmpty();

		RuleFor(x => x.ApiUsageDemoSubscription)
			.Must((value) => RateLimiterConfiguration.TryParse(value, out var _))
			.WithMessage($"Invalid configuration for '{nameof(RateLimiterConfigurationOptions.ApiUsageDemoSubscription)}' rate limiter. Please check the configuration."); 
	}
}
