using Binacle.Net.Api.ServiceModule.Data.Entities;

namespace Binacle.Net.Api.ServiceModule.Models;


internal record UserActionRequest(string Email, string Password);
internal record UserActionResult(bool Success, UserEntity? User = null, string? Message = null, UserActionResultType? ResultType = null);

internal enum UserActionResultType
{
	MalformedRequest,
	Unauthorized,
	Conflict,
	NotFound,
	Unspecified
}
