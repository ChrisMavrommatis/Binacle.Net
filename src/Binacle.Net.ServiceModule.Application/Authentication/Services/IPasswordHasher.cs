namespace Binacle.Net.ServiceModule.Application.Authentication.Services;

public interface IPasswordHasher
{
	bool PasswordMatches(string password, string passwordHash);
	
	string CreateHash(string password);
}
