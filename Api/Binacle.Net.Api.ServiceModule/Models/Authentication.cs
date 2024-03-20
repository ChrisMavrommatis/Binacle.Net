using Binacle.Net.Api.ServiceModule.Data.Entities;

namespace Binacle.Net.Api.ServiceModule.Models;

internal record AuthenticationRequest(string Email, string Password);

internal record AuthenticationResult(bool Success, AuthenticationFailedResultReason? Reason = null, UserEntity? User = null);

internal enum AuthenticationFailedResultReason
{
	InvalidCredentials,
	Unknown
}
