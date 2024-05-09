using Binacle.Net.Api.ServiceModule.Models;
using FluentValidation;

namespace Binacle.Net.Api.ServiceModule.Validators;

internal class PasswordValidator : AbstractValidator<IWithPassword>
{
	public PasswordValidator()
	{
		RuleFor(x => x.Password).NotNull().NotEmpty().MinimumLength(10);
	}
}
