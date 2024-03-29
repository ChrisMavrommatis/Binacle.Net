using Binacle.Net.Api.ServiceModule.ApiModels.Validators;
using FluentValidation;

namespace Binacle.Net.Api.ServiceModule.ApiModels.Requests;

internal class CreateApiUserRequestValidator : AbstractValidator<CreateApiUserRequest>
{
	public CreateApiUserRequestValidator()
	{
		Include(x => new AuthenticationInformationValidator());
	}
}
