using System.ComponentModel.DataAnnotations;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Contracts.Common.Interfaces;
using FluentValidation;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class CreateAccountRequest : IWithUsername, IWithPassword, IWithEmail
{
	[Required] public string Username { get; set; }

	[Required] public string Password { get; set; }

	[Required] public string Email { get; set; }
}


internal class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
	{
		public CreateAccountRequestValidator()
		{
			Include(x => new UsernameValidator());
			Include(x => new PasswordValidator());
			Include(x => new EmailValidator());
		}
	}
	
	internal class CreateAccountRequestExample : ISingleOpenApiExamplesProvider<CreateAccountRequest>
	{
		public IOpenApiExample<CreateAccountRequest> GetExample()
		{
			return OpenApiExample.Create(
				"createAccount",
				"Create Account",
				new CreateAccountRequest
				{
					Username = "test_user",
					Email = "user@domain.test",
					Password = "testpassword"
				}
			);
		}
	}
	
	internal class CreateAccountRequestValidationProblemResponseExample : ValidationProblemResponseExample
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









