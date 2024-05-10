namespace Binacle.Net.Api.ServiceModule.Domain.Users.Models;

public record UpdateUserRequest(string Email, string Group, bool IsActive);
