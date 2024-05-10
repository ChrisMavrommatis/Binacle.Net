using Binacle.Net.Api.ServiceModule.Domain.Validators;
using FluentValidation;

namespace Binacle.Net.Api.ServiceModule.v0.Requests.Validators;

internal class TokenRequestValidator : AbstractValidator<TokenRequest>
{
	public TokenRequestValidator()
	{
		Include(x => new AuthenticationInformationValidator());
	}
}
