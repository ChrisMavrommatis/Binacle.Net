using Binacle.Net.ServiceModule.Domain.Validators;
using FluentValidation;

namespace Binacle.Net.ServiceModule.v0.Requests.Validators;

internal class CreateApiUserRequestValidator : AbstractValidator<CreateApiUserRequest>
{
	public CreateApiUserRequestValidator()
	{
		Include(x => new AuthenticationInformationValidator());
	}
}
