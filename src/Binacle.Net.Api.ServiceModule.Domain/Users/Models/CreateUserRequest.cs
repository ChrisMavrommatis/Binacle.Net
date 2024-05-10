using Binacle.Net.Api.ServiceModule.Domain.Users.Entities;

namespace Binacle.Net.Api.ServiceModule.Domain.Users.Models;

public record CreateUserRequest(string Email, string Password, string Group = UserGroups.Users);
