using Binacle.Net.ServiceModule.Domain.Validators;
using FluentValidation;

namespace Binacle.Net.ServiceModule.v0.Requests.Validators;

internal class DeleteApiUserRequestValidator : AbstractValidator<DeleteApiUserRequest>
{
	public DeleteApiUserRequestValidator()
	{
		Include(x => new EmailValidator());
	}
}
