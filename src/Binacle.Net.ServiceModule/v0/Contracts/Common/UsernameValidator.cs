using Binacle.Net.ServiceModule.v0.Contracts.Common.Interfaces;
using FluentValidation;

namespace Binacle.Net.ServiceModule.v0.Contracts.Common;

internal class UsernameValidator : AbstractValidator<IWithUsername>
{
	public UsernameValidator()
	{
		RuleFor(x => x.Username).NotNull().NotEmpty().MinimumLength(5);
	}
}
