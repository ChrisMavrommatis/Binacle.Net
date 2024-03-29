using FluentValidation;

namespace Binacle.Net.Api.ServiceModule.ApiModels.Validators;

internal class PasswordValidator : AbstractValidator<IWithPassword>
{
	public PasswordValidator()
	{
		RuleFor(x => x.Password).NotNull().NotEmpty().MinimumLength(10);
	}
}
