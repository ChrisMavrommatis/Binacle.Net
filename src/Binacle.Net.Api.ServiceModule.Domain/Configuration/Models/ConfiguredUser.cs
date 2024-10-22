using Binacle.Net.Api.ServiceModule.Domain.Models;

namespace Binacle.Net.Api.ServiceModule.Domain.Configuration.Models;

public class ConfiguredUser : IAuthenticationInformation
{
	public required string Email { get; set; }
	public required string Password { get; set; }
}

