using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using FluentValidation;
using FluentValidation.Validators;

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
			.WithMessage($"'{nameof(UpdateAccountRequest.Status)}' is required and must be one of the following values: {string.Join(", ", accountStatusValues)}");

		var accountRoleValues = Enum.GetValues<AccountRole>();

		RuleFor(x => x.Role)
			.NotNull()
			.WithMessage($"'{nameof(UpdateAccountRequest.Role)}' is required and must be one of the following values: {string.Join(", ", accountRoleValues)}");

	}
}
