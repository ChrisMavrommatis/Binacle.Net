using Binacle.Net.ServiceModule.Domain.Validators;
using FluentValidation;

namespace Binacle.Net.ServiceModule.v0.Requests.Validators;

internal class TokenRequestValidator : AbstractValidator<TokenRequest>
{
	public TokenRequestValidator()
	{
		Include(x => new AuthenticationInformationValidator());
	}
}
