using Binacle.Net.ServiceModule.Models;
using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Auth;

internal class TokenResponse
{
	public required string TokenType { get; set; }
	public required string AccessToken { get; set; }
	public required int ExpiresIn { get; set; }
	public string? RefreshToken { get; set; }

	public static TokenResponse Create(Token token)
	{
		return new TokenResponse
		{
			TokenType = token.TokenType,
			AccessToken = token.TokenValue,
			ExpiresIn = token.ExpiresIn,
		};
	}
}

internal class TokenResponseExample : ISingleOpenApiExamplesProvider<TokenResponse>
{
	public IOpenApiExample<TokenResponse> GetExample()
	{
		return OpenApiExample.Create(
			"successfulTokenResponse",
			"Successful Token Response",
			new TokenResponse
			{
				AccessToken =
					"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c",
				TokenType = "Bearer",
				ExpiresIn = 3600
			}
		);
	}
}
