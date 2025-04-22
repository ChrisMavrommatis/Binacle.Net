using Binacle.Net.ServiceModule.Application.Authentication.Services;

namespace Binacle.Net.ServiceModule.Infrastructure.Authentication.Services;

internal class PasswordHasher : IPasswordHasher
{
	public bool PasswordMatches(string password, string passwordHash)
	{
		return passwordHash == password;
	}

	public string CreateHash(string password)
	{
		return password;
	}
}
