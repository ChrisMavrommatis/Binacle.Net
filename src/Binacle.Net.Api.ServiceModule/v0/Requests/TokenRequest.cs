﻿using Binacle.Net.Api.ServiceModule.Domain.Models;

namespace Binacle.Net.Api.ServiceModule.v0.Requests;

internal class TokenRequest : IAuthenticationInformation
{
	public string Email { get; set; }
	public string Password { get; set; }
}
