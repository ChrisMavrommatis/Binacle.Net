using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class CreateAccountRequestExample : ISingleOpenApiExamplesProvider<CreateAccountRequest>
{
	public IOpenApiExample<CreateAccountRequest> GetExample()
	{
		return OpenApiExample.Create(
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
