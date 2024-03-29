﻿using FluentValidation;

namespace Binacle.Net.Api.ServiceModule.ApiModels.Validators;

internal class AuthenticationInformationValidator : AbstractValidator<IAuthenticationInformation>
{
	public AuthenticationInformationValidator()
	{
		Include(x => new EmailValidator());
		Include(x => new PasswordValidator());
	}
}
