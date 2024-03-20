using Binacle.Net.Api.ServiceModule.Configuration.Models;
using FluentValidation;

namespace Binacle.Net.Api.ServiceModule.Configuration.Validators;

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
