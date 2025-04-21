using Binacle.Net.ServiceModule.v0.Contracts.Common;
using FluentValidation;

namespace Binacle.Net.ServiceModule.v0.Contracts.Auth;

internal class TokenRequestValidator : AbstractValidator<TokenRequest>
{
	public TokenRequestValidator()
	{
		Include(x => new PasswordValidator());
	}
}
