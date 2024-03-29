
namespace Binacle.Net.Api.ServiceModule.ApiModels.Requests;

internal class DeleteApiUserRequest : IWithEmail
{
	public string Email { get; set; }
}
