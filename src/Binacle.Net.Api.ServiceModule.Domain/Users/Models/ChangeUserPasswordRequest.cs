namespace Binacle.Net.Api.ServiceModule.Domain.Users.Models;

public record ChangeUserPasswordRequest(string Email, string NewPassword);
