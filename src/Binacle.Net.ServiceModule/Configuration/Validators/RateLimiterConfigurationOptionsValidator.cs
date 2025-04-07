using Binacle.Net.ServiceModule.Configuration.Models;
using FluentValidation;

namespace Binacle.Net.ServiceModule.Configuration.Validators;

internal class RateLimiterConfigurationOptionsValidator : AbstractValidator<RateLimiterConfigurationOptions>
{
	public RateLimiterConfigurationOptionsValidator()
	{
		RuleFor(x => x.Anonymous).NotNull().NotEmpty();

		RuleFor(x => x.Anonymous).Must((value) =>
		{
			try
			{
				var result = RateLimiterConfigurationOptions.ParseConfiguration(value);
				return result is not null;
			}
			catch(Exception _)
			{
				return false;
			}	
		});
	}
}
