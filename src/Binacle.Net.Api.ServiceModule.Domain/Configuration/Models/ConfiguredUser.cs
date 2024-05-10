using Binacle.Net.Api.ServiceModule.Domain.Models;

namespace Binacle.Net.Api.ServiceModule.Domain.Configuration.Models;

public class ConfiguredUser : IAuthenticationInformation
{
	public string Email { get; set; }
	public string Password { get; set; }
}

