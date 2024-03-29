using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.Api.ServiceModule.ApiModels.Requests;

internal class ChangeApiUserPasswordRequestWithBody : IWithEmail
{
	[FromRoute]
	public string Email { get; set; }

	[FromBody]
	public ChangeApiUserPasswordRequest Body { get; set; }

}

internal class ChangeApiUserPasswordRequest : IWithPassword
{
	public string Password { get; set; }

}
