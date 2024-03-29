
namespace Binacle.Net.Api.ServiceModule.ApiModels.Requests;

internal class TokenRequest : IAuthenticationInformation
{
	public string Email { get; set; }
	public string Password { get; set; }
}
