using Binacle.Net.ServiceModule.Domain.Configuration.Models;
using Binacle.Net.ServiceModule.Domain.Validators;
using FluentValidation;

namespace Binacle.Net.ServiceModule.Configuration.Validators;

internal class UserOptionsValidator: AbstractValidator<UserOptions>
{
	public UserOptionsValidator()
	{
		RuleFor(x => x.DefaultAdminUser)
			.NotNull()
			.NotEmpty()
			.Must(x => x!.Contains(":"));


		RuleFor(x => x.GetParsedDefaultAdminUser())
			.NotNull()
			.SetValidator(x => new AuthenticationInformationValidator());
	}
}
