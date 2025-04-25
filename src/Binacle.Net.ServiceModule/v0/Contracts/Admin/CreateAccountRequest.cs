using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Contracts.Common.Interfaces;
using FluentValidation;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class CreateAccountRequest : IWithUsername, IWithPassword, IWithEmail
{
	public string Username { get; set; }
	public string Password { get; set; }
	public string Email { get; set; }
	
	internal class Validator : AbstractValidator<CreateAccountRequest>
	{
		public Validator()
		{
			Include(x => new UsernameValidator());
			Include(x => new PasswordValidator());
			Include(x => new EmailValidator());
		}
	}
	
	internal class Example : ISingleOpenApiExamplesProvider<CreateAccountRequest>
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
	
	internal class ErrorResponseExamples : IMultipleOpenApiExamplesProvider<ErrorResponse>
	{
		public IEnumerable<IOpenApiExample<ErrorResponse>> GetExamples()
		{
			yield return OpenApiExample.Create(
				"validationError",
				"Validation Error",
				"Example response with validation errors",
				ErrorResponse.ValidationError(
				[
					"'Email' is not a valid email address.",
					"The length of 'Password' must be at least 10 characters. You entered 8 characters."
				])
			);

			yield return OpenApiExample.Create(
				"otherError",
				"Other Error",
				"Example response when something went wrong",
				ErrorResponse.Create("Could not create user")
			);
		}
	}
}








