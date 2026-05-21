using Binacle.Net.ServiceModule.v0.Contracts.Common.Interfaces;
using FluentValidation;

namespace Binacle.Net.ServiceModule.v0.Contracts.Common;

internal class PasswordValidator : AbstractValidator<IWithPassword>
{
	public PasswordValidator()
	{
		RuleFor(x => x.Password).NotNull().NotEmpty().MinimumLength(10);
	}
}
