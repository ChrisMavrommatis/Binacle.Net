using Binacle.Net.ServiceModule.Configuration.Models;
using Binacle.Net.ServiceModule.Helpers;
using FluentValidation;

namespace Binacle.Net.ServiceModule.Configuration.Validators;

internal class RateLimiterConfigurationOptionsValidator : AbstractValidator<RateLimiterConfigurationOptions>
{
	public RateLimiterConfigurationOptionsValidator()
	{
		RuleFor(x => x.Anonymous).NotNull().NotEmpty();

		RuleFor(x => x.Anonymous)
			.Must((value) => RateLimiterConfigurationParser.TryParse(value, out var _))
			.WithMessage("Invalid configuration for Anonymous rate limiter. Please check the configuration."); 
	}
}
