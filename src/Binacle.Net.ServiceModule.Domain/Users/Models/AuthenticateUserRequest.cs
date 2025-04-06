namespace Binacle.Net.ServiceModule.Domain.Users.Models;

public record AuthenticateUserRequest(string Email, string Password);
