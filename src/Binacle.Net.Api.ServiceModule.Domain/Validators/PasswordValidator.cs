using Binacle.Net.Api.ServiceModule.Domain.Models;
using FluentValidation;

namespace Binacle.Net.Api.ServiceModule.Domain.Validators;

public class PasswordValidator : AbstractValidator<IWithPassword>
{
	public PasswordValidator()
	{
		RuleFor(x => x.Password).NotNull().NotEmpty().MinimumLength(10);
	}
}
