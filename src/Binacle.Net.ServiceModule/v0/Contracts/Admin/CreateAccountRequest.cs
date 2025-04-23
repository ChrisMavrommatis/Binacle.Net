using Binacle.Net.ServiceModule.v0.Contracts.Common.Interfaces;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class CreateAccountRequest : IWithUsername, IWithPassword, IWithEmail
{
	public string Username { get; set; }
	public string Password { get; set; }
	public string Email { get; set; }
}
