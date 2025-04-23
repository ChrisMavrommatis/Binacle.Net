using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using FluentValidation;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class UpdateAccountRequestValidator : AbstractValidator<UpdateAccountRequest>
{
	public UpdateAccountRequestValidator()
	{
		Include(x => new UsernameValidator());
		Include(x => new EmailValidator());
		Include(x => new PasswordValidator());
		
		var accountStatusValues = Enum.GetValues<AccountStatus>();
		
		RuleFor(x => x.Status)
			.NotNull()
			.WithMessage($"Is required and must be one of the following values: {string.Join(", ", accountStatusValues)}");

		// "Is required and must be one of the following values: Active, Inactive, Suspended",
		// "'Role' must not be empty."
		RuleFor(x => x.Role)
			.NotNull()
			.IsInEnum();
	}
}

