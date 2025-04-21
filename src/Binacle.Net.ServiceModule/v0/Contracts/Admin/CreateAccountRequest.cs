using Binacle.Net.ServiceModule.v0.Contracts.Common.Interfaces;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class CreateAccountRequest : IWithPassword, IWithEmail
{
	public string Username { get; set; }
	public string Password { get; set; }
	public string Email { get; set; }
}
