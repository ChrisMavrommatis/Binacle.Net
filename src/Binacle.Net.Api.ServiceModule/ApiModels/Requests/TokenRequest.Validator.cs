using Binacle.Net.Api.ServiceModule.ApiModels.Validators;
using FluentValidation;

namespace Binacle.Net.Api.ServiceModule.ApiModels.Requests;

internal class TokenRequestValidator : AbstractValidator<TokenRequest>
{
	public TokenRequestValidator()
	{
		Include(x => new AuthenticationInformationValidator());
	}
}
