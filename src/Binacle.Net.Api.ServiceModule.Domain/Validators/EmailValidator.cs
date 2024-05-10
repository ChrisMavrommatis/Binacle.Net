using Binacle.Net.Api.ServiceModule.Domain.Models;
using FluentValidation;

namespace Binacle.Net.Api.ServiceModule.Domain.Validators;

public class EmailValidator : AbstractValidator<IWithEmail>
{
	public EmailValidator()
	{
		RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress();
	}
}
