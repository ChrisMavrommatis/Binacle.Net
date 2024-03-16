using Binacle.Net.Api.Users.Requests;
using FluentValidation;

namespace Binacle.Net.Api.Users.Validators;

internal class CreateApiUserRequestValidator: AbstractValidator<CreateApiUserRequest>
{
	public CreateApiUserRequestValidator()
	{
		RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress();
		RuleFor(x => x.Password).NotNull().NotEmpty().MinimumLength(10);
	}
}
