using Binacle.Net.Api.ServiceModule.ApiModels.Validators;
using FluentValidation;

namespace Binacle.Net.Api.ServiceModule.ApiModels.Requests;

internal class ChangeApiUserPasswordRequestValidator : AbstractValidator<ChangeApiUserPasswordRequestWithBody>
{
	public ChangeApiUserPasswordRequestValidator()
	{
		Include(x => new EmailValidator());
		RuleFor(x => x.Body).SetValidator(x => new PasswordValidator());
	}
}
