using Binacle.Net.Api.ServiceModule.Configuration.Models;
using FluentValidation;

namespace Binacle.Net.Api.ServiceModule.Configuration.Validators;

internal class RateLimiterConfigurationOptionsValidator : AbstractValidator<RateLimiterConfigurationOptions>
{
	public RateLimiterConfigurationOptionsValidator()
	{
		RuleFor(x => x.Anonymous).NotNull().NotEmpty();

		RuleFor(x => x.Anonymous).Must((value) => RateLimiterConfigurationOptions.ParseConfiguration(value) is not null);
	}
}
