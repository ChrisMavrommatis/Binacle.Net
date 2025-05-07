using System.ComponentModel.DataAnnotations;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Contracts.Common.Interfaces;
using FluentValidation;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Auth;

internal class TokenRequest : IWithPassword
{
	public required string Username { get; set; }
	public required string Password { get; set; }
}

internal class TokenRequestValidator : AbstractValidator<TokenRequest>
{
	public TokenRequestValidator()
	{
		Include(x => new PasswordValidator());
	}
}

internal class TokenRequestExample : ISingleOpenApiExamplesProvider<TokenRequest>
{
	public IOpenApiExample<TokenRequest> GetExample()
	{
		return OpenApiExample.Create(
			"authToken",
			"Auth Token",
			new TokenRequest
			{
				Username = "test_user", 
				Password = "testpassword"
			}
		);
	}
}

internal class TokenRequestValidationProblemExample : ValidationProblemResponseExample
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
