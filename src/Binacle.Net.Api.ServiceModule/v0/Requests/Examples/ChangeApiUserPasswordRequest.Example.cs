using ChrisMavrommatis.SwaggerExamples;

namespace Binacle.Net.Api.ServiceModule.v0.Requests.Examples;

internal class ChangeApiUserPasswordRequestExample : SingleSwaggerExamplesProvider<ChangeApiUserPasswordRequest>
{
	public override ChangeApiUserPasswordRequest GetExample()
	{
		return new ChangeApiUserPasswordRequest
		{
			Password = "newpassword"
		};
	}
}
