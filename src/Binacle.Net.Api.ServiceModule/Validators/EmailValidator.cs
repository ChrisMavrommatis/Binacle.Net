using Binacle.Net.Api.ServiceModule.Models;
using FluentValidation;

namespace Binacle.Net.Api.ServiceModule.Validators;

internal class EmailValidator : AbstractValidator<IWithEmail>
{
	public EmailValidator()
	{
		RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress();
	}
}
