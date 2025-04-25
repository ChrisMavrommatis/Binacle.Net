using Binacle.Net.ServiceModule.Domain.Common.Services;

namespace Binacle.Net.ServiceModule.Infrastructure.Common.Services;

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
