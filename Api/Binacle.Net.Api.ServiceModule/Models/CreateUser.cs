
namespace Binacle.Net.Api.ServiceModule.Models;


internal record CreateUserRequest(string Email, string Password);
internal record CreateUserResult(bool Success, CreateUserFailedResultReason? Reason = null);

internal enum CreateUserFailedResultReason
{
	AlreadyExists,
	InvalidCredentials,
	Unknown
}

