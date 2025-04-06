using Binacle.Net.ServiceModule.Domain.Models;
using FluentValidation;

namespace Binacle.Net.ServiceModule.Domain.Validators;

public class PasswordValidator : AbstractValidator<IWithPassword>
{
	public PasswordValidator()
	{
		RuleFor(x => x.Password).NotNull().NotEmpty().MinimumLength(10);
	}
}
