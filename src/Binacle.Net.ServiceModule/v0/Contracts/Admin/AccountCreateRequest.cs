using System.ComponentModel.DataAnnotations;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Contracts.Common.Interfaces;
using FluentValidation;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class AccountCreateRequest : IWithUsername, IWithPassword, IWithEmail
{
	[Required] 
	public string Username { get; set; } = null!;

	[Required] 
	public string Password { get; set; } = null!;

	[Required] 
	public string Email { get; set; } = null!;
}

internal class AccountCreateRequestValidator : AbstractValidator<AccountCreateRequest>
{
	public AccountCreateRequestValidator()
	{
		Include(x => new UsernameValidator());
		Include(x => new PasswordValidator());
		Include(x => new EmailValidator());
	}
}

internal class AccountCreateRequestExample : ISingleOpenApiExamplesProvider<AccountCreateRequest>
{
	public IOpenApiExample<AccountCreateRequest> GetExample()
	{
		return OpenApiExample.Create(
			"accountCreate",
			"Account Create",
			new AccountCreateRequest
			{
				Username = "test_user",
				Email = "user@domain.test",
				Password = "testpassword"
			}
		);
	}
}

internal class AccountCreateValidationProblemExample : ValidationProblemResponseExample
{
	public override Dictionary<string, string[]> GetErrors()
	{
		return new Dictionary<string, string[]>()
		{
			{ "Email", ["'Email' is not a valid email address."] },
			{ "Password", ["The length of 'Password' must be at least 10 characters. You entered 8 characters."] }
		};
	}
}
