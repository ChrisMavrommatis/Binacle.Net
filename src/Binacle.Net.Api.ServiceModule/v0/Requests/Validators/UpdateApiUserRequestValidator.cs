using Binacle.Net.Api.ServiceModule.Domain.Validators;
using FluentValidation;

namespace Binacle.Net.Api.ServiceModule.v0.Requests.Validators;

internal class UpdateApiUserRequestValidator : AbstractValidator<UpdateApiUserRequestWithBody>
{
	public UpdateApiUserRequestValidator()
	{
		Include(x => new EmailValidator());
		RuleFor(x => x.Body).NotNull();
	}
}
