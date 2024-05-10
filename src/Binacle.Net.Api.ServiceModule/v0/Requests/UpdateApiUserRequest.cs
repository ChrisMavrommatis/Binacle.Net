using Binacle.Net.Api.ServiceModule.Domain.Models;
using Binacle.Net.Api.ServiceModule.Models;
using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.Api.ServiceModule.v0.Requests;

internal class UpdateApiUserRequestWithBody: IWithEmail
{
	[FromRoute]
	public string Email { get; set; }

	[FromBody]
	public ChangeApiUserPasswordRequest Body { get; set; }
}

internal class UpdateApiUserRequest
{
	public UserType UserType { get; set; }

	public bool IsActive { get; set; }
}

