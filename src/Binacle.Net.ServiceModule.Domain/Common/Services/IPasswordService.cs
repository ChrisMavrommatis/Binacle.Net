using Binacle.Net.ServiceModule.Domain.Common.Models;

namespace Binacle.Net.ServiceModule.Domain.Common.Services;

public interface IPasswordService
{
	Password Create(string plainTextPassword, string? type = null);
	bool PasswordMatches(Password storedPassword, string plainTextPassword);
	
}
