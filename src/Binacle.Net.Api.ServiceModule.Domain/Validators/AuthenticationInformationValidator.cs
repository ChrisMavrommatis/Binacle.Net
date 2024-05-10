using Binacle.Net.Api.ServiceModule.Domain.Models;
using FluentValidation;

namespace Binacle.Net.Api.ServiceModule.Domain.Validators;

public class AuthenticationInformationValidator : AbstractValidator<IAuthenticationInformation>
{
	public AuthenticationInformationValidator()
	{
		Include(x => new EmailValidator());
		Include(x => new PasswordValidator());
	}
}
