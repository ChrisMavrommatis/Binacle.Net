using Binacle.Net.Api.ServiceModule.Models;
using FluentValidation;

namespace Binacle.Net.Api.ServiceModule.Validators;

internal class AuthenticationInformationValidator : AbstractValidator<IAuthenticationInformation>
{
	public AuthenticationInformationValidator()
	{
		Include(x => new EmailValidator());
		Include(x => new PasswordValidator());
	}
}
