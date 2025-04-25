using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Contracts.Common.Interfaces;
using FluentValidation;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Auth;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

internal class TokenRequest : IWithPassword
{
	public string Username { get; set; }
	public string Password { get; set; }

	internal class Validator : AbstractValidator<TokenRequest>
	{
		public Validator()
		{
			Include(x => new PasswordValidator());
		}
	}

	internal class Example : ISingleOpenApiExamplesProvider<TokenRequest>
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
}
