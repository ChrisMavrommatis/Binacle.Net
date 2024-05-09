using Binacle.Net.Api.ServiceModule.Models;

namespace Binacle.Net.Api.ServiceModule.v0.Requests;

internal class DeleteApiUserRequest : IWithEmail
{
	public string Email { get; set; }
}
