using Binacle.Net.Api.ServiceModule.Domain.Models;

namespace Binacle.Net.Api.ServiceModule.v0.Requests;

internal class DeleteApiUserRequest : IWithEmail
{
	public string Email { get; set; }
}
