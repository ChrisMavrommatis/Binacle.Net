namespace Binacle.Net.ServiceModule.Domain.Common.Services;

public interface IPasswordHasher
{
	bool PasswordMatches(string password, string passwordHash);
	
	string CreateHash(string password);
}
