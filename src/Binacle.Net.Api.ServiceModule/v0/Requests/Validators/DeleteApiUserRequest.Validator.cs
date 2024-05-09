using Binacle.Net.Api.ServiceModule.Validators;
using FluentValidation;

namespace Binacle.Net.Api.ServiceModule.v0.Requests.Validators;

internal class DeleteApiUserRequestValidator : AbstractValidator<DeleteApiUserRequest>
{
	public DeleteApiUserRequestValidator()
	{
		Include(x => new EmailValidator());
	}
}
