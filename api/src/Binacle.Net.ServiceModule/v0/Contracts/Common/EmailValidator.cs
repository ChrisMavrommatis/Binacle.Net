using Binacle.Net.ServiceModule.v0.Contracts.Common.Interfaces;
using FluentValidation;

namespace Binacle.Net.ServiceModule.v0.Contracts.Common;

internal class EmailValidator : AbstractValidator<IWithEmail>
{
	public EmailValidator()
	{
		RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress();
	}
}
