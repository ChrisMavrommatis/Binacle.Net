using Binacle.Net.Api.ServiceModule.ApiModels.Validators;
using FluentValidation;

namespace Binacle.Net.Api.ServiceModule.ApiModels.Requests;

internal class DeleteApiUserRequestValidator : AbstractValidator<DeleteApiUserRequest>
{
	public DeleteApiUserRequestValidator()
	{
		Include(x => new EmailValidator());
	}
}
