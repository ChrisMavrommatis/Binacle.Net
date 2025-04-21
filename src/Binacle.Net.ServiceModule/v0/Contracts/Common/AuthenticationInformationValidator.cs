using Binacle.Net.ServiceModule.v0.Contracts.Common.Interfaces;
using FluentValidation;

namespace Binacle.Net.ServiceModule.v0.Contracts.Common;

public class AuthenticationInformationValidator : AbstractValidator<IWithAuthenticationInformation>
{
	public AuthenticationInformationValidator()
	{
		Include(x => new EmailValidator());
		Include(x => new PasswordValidator());
	}
}
