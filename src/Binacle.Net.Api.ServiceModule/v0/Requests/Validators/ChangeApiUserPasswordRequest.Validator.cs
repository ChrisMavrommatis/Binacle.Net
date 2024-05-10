using Binacle.Net.Api.ServiceModule.Domain.Validators;
using FluentValidation;

namespace Binacle.Net.Api.ServiceModule.v0.Requests.Validators;

internal class ChangeApiUserPasswordRequestValidator : AbstractValidator<ChangeApiUserPasswordRequestWithBody>
{
	public ChangeApiUserPasswordRequestValidator()
	{
		Include(x => new EmailValidator());
		RuleFor(x => x.Body).SetValidator(x => new PasswordValidator());
	}
}
