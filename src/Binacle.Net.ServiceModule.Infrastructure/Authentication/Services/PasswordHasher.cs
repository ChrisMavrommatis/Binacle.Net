using Binacle.Net.ServiceModule.Application.Authentication.Services;

namespace Binacle.Net.ServiceModule.Infrastructure.Authentication.Services;

internal class PasswordHasher : IPasswordHasher
{
	public bool PasswordMatches(string password, string passwordHash)
	{
		throw new NotImplementedException();
	}

	public string CreateHash(string password)
	{
		throw new NotImplementedException();
	}
}
