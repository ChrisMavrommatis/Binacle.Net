using System.Linq.Expressions;
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
		// One chain per setting, stopping at the first failure. Split across two RuleFor blocks the operator got
		// the same "must not be empty" twice plus a third line for the parse — three errors describing one
		// missing setting.
		RuleForLimiter(x => x.ApiUsageAnonymous);
		RuleForLimiter(x => x.AuthToken);
		RuleForLimiter(x => x.ApiUsageDemoSubscription);
	}

	// The limiters are configured as strings, so the message has to carry the format — "check the configuration"
	// leaves an operator with a rejected file and nowhere to look.
	private void RuleForLimiter(Expression<Func<RateLimiterConfigurationOptions, string?>> setting)
	{
		RuleFor(setting)
			.Cascade(CascadeMode.Stop)
			.NotEmpty()
			.Must(value => RateLimiterConfiguration.TryParse(value, out _))
			.WithMessage(
				"'{PropertyName}' is not a valid rate limiter. Use 'NoLimiter::0', "
				+ "'FixedWindow::PermitLimit/WindowInSeconds' (for example 'FixedWindow::5/60'), or "
				+ "'SlidingWindow::PermitLimit/WindowInSeconds-Segments' (for example 'SlidingWindow::5/60-4'). "
				+ "You entered '{PropertyValue}'."
			);
	}
}
