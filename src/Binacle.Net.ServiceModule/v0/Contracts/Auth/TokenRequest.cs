

namespace Binacle.Net.ServiceModule.v0.Contracts.Auth;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

internal class TokenRequest 
{
	public string Email { get; set; }
	public string Password { get; set; }
}
