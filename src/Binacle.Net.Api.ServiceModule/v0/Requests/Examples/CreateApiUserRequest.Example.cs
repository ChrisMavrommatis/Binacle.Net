using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.Api.ServiceModule.v0.Requests.Examples;

internal class CreateApiUserRequestExample : ISingleOpenApiExamplesProvider<CreateApiUserRequest>
{
	public IOpenApiExample<CreateApiUserRequest> GetExample()
	{
		return OpenApiExample.Create(
			"Create API User",
			new CreateApiUserRequest
			{
				Email = "useremail@domain.com",
				Password = "userspassword"
			}
		);
	}
}
