using Binacle.Net.ServiceModule.Domain.Users.Entities;

namespace Binacle.Net.ServiceModule.Domain.Users.Models;

public record CreateUserRequest(string Email, string Password, string Group = UserGroups.Users);
