using Binacle.Net.ServiceModule.Domain.Models;

namespace Binacle.Net.ServiceModule.Domain.Configuration.Models;

public class ConfiguredUser : IAuthenticationInformation
{
	public required string Email { get; set; }
	public required string Password { get; set; }
}

