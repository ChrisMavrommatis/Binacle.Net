using Binacle.Net.Api.ServiceModule.Domain.Models;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Binacle.Net.Api.ServiceModule.v0.Requests;


internal class DeleteApiUserRequest : IWithEmail
{
	public string Email { get; set; }
}
