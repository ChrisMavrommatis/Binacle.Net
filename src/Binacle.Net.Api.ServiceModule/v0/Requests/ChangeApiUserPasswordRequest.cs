using Binacle.Net.Api.ServiceModule.Models;
using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.Api.ServiceModule.v0.Requests;

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
