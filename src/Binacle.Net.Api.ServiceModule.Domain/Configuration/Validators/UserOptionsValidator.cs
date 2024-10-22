using Binacle.Net.Api.ServiceModule.Domain.Configuration.Models;
using Binacle.Net.Api.ServiceModule.Domain.Validators;
using FluentValidation;

namespace Binacle.Net.Api.ServiceModule.Configuration.Validators;

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
