using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using FluentValidation;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class PartialUpdateAccountRequestValidator : AbstractValidator<PartialUpdateAccountRequest>
{
	public PartialUpdateAccountRequestValidator()
	{
		When(x => !string.IsNullOrEmpty(x.Email), () =>
		{
			RuleFor(x => x)
				.SetValidator(new EmailValidator());
		});

		When(x => !string.IsNullOrEmpty(x.Password), () =>
		{
			RuleFor(x => x)
				.SetValidator(new PasswordValidator());
		});

		When(x => !string.IsNullOrEmpty(x.Username), () =>
		{
			RuleFor(x => x)
				.SetValidator(new UsernameValidator());
		});

		RuleFor(x => x)
			.Must(x =>
				!string.IsNullOrEmpty(x.Email)
				|| !string.IsNullOrEmpty(x.Password)
				|| !string.IsNullOrEmpty(x.Username)
				|| x.Status.HasValue
				|| x.Role.HasValue
			).WithMessage(
				"At least one field must be provided for update."
			);
	}
}
