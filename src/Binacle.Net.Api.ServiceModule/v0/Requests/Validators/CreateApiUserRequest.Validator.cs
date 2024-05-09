using Binacle.Net.Api.ServiceModule.Validators;
using FluentValidation;

namespace Binacle.Net.Api.ServiceModule.v0.Requests.Validators;

internal class CreateApiUserRequestValidator : AbstractValidator<CreateApiUserRequest>
{
	public CreateApiUserRequestValidator()
	{
		Include(x => new AuthenticationInformationValidator());
	}
}
