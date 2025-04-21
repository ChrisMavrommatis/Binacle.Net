using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Auth;

internal class TokenRequestExample : ISingleOpenApiExamplesProvider<TokenRequest>
{
	public IOpenApiExample<TokenRequest> GetExample()
	{
		return OpenApiExample.Create(
			"Auth Token",
			new TokenRequest
			{
				Username = "test_user",
				Password = "testpassword"
			}
		);
	}
}
