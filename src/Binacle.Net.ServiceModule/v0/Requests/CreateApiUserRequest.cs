using Binacle.Net.ServiceModule.Domain.Models;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Binacle.Net.ServiceModule.v0.Requests;

internal class CreateApiUserRequest : IAuthenticationInformation
{
	public string Email { get; set; }
	public string Password { get; set; }
}
