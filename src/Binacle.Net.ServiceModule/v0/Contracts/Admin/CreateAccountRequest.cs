using Binacle.Net.ServiceModule.v0.Contracts.Common.Interfaces;

namespace Binacle.Net.ServiceModule.v0.Contracts.Admin;

internal class CreateAccountRequest : IWithAuthenticationInformation
{
	public string Email { get; set; }
	public string Password { get; set; }
}
