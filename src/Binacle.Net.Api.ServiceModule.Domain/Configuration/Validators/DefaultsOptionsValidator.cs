using Binacle.Net.Api.ServiceModule.Domain.Configuration.Models;
using Binacle.Net.Api.ServiceModule.Domain.Validators;
using FluentValidation;

namespace Binacle.Net.Api.ServiceModule.Configuration.Validators;

internal class DefaultsOptionsValidator: AbstractValidator<DefaultsOptions>
{
	public DefaultsOptionsValidator()
	{
		RuleFor(x => x.AdminUser)
			.NotNull()
			.NotEmpty()
			.Must(x => x.Contains(":"));


		RuleFor(x => x.GetParsedAdminUser())
			.NotNull()
			.SetValidator(x => new AuthenticationInformationValidator());
			

	}
}
