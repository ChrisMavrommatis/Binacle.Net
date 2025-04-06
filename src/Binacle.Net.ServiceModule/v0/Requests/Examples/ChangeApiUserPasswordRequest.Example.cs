using OpenApiExamples;
using OpenApiExamples.Abstractions;

namespace Binacle.Net.ServiceModule.v0.Requests.Examples;

internal class ChangeApiUserPasswordRequestExample : ISingleOpenApiExamplesProvider<ChangeApiUserPasswordRequest>
{
	public IOpenApiExample<ChangeApiUserPasswordRequest> GetExample()
	{
		return OpenApiExample.Create(
			"Change Password",
			new ChangeApiUserPasswordRequest
			{
				Password = "newpassword"
			}
		);
	}
}
