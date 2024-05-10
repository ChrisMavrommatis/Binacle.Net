﻿using Binacle.Net.Api.ServiceModule.Models;

namespace Binacle.Net.Api.ServiceModule.v0.Requests;

internal class CreateApiUserRequest : IAuthenticationInformation
{
	public string Email { get; set; }
	public string Password { get; set; }
}