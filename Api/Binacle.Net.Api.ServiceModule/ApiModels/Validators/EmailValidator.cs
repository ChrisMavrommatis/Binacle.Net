using FluentValidation;

namespace Binacle.Net.Api.ServiceModule.ApiModels.Validators;

internal class EmailValidator : AbstractValidator<IWithEmail>
{
	public EmailValidator()
	{
		RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress();
	}
}
