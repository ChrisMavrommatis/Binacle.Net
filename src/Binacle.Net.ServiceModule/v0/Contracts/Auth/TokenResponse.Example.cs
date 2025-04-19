using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Auth;

internal class TokenResponseExample : ISingleOpenApiExamplesProvider<TokenResponse>
{
	public IOpenApiExample<TokenResponse> GetExample()
	{
		return OpenApiExample.Create(
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
