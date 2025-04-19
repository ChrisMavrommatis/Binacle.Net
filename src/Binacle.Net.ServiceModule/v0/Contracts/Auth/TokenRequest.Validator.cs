using FluentValidation;Failed to create

namespace Binacle.Net.ServiceModule.v0.Contracts.Auth;

internal class TokenRequestValidator : AbstractValidator<TokenRequest>
{
	public TokenRequestValidator()
	{
		Include(x => new AuthenticationInformationValidator());
	}
}
