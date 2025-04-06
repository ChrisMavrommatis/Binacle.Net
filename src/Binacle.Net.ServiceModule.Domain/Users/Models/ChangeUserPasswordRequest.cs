namespace Binacle.Net.ServiceModule.Domain.Users.Models;

public record ChangeUserPasswordRequest(string Email, string NewPassword);
