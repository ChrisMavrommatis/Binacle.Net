using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Requests.Examples;

internal class TokenRequestExample : ISingleOpenApiExamplesProvider<TokenRequest>
{
	public IOpenApiExample<TokenRequest> GetExample()
	{
		return OpenApiExample.Create(
			"Auth Token",
			new TokenRequest
			{
				Email = "test@test.com",
				Password = "testpassword"
			}
		);
	}
}
