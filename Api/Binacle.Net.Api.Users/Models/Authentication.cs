using Binacle.Net.Api.Users.Data.Entities;

namespace Binacle.Net.Api.Users.Models;

internal record AuthenticationRequest(string Email, string Password);

internal record AuthenticationResult(bool Success, AuthenticationFailedResultReason? Reason = null, UserEntity? User = null);

internal enum AuthenticationFailedResultReason
{
	InvalidCredentials,
	Unknown
}
