using Binacle.Net.ServiceModule.v0.Contracts.Common;
using FluentValidation;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
{
	public CreateAccountRequestValidator()
	{
		Include(x => new PasswordValidator());
		Include(x => new EmailValidator());
	}
}
