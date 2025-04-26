using System.Security.Cryptography;
using System.Text;
using Binacle.Net.ServiceModule.Domain.Common.Models;
using Binacle.Net.ServiceModule.Domain.Common.Services;

namespace Binacle.Net.ServiceModule.Infrastructure.Common.Services;

internal class Sha256PasswordHasher : IPasswordHasher
{
	public const string Key = "SHA256";

	public string Type => Key;

	private const int _saltSize = 16; // 128 Bits
	
	public string GenerateSalt()
	{
		return Convert.ToBase64String(RandomNumberGenerator.GetBytes(_saltSize));
	}
	public Password Create(string plainTextPassword, string? salt = null)
	{
		using var sha256 = SHA256.Create();
		
		byte[] passwordBytes = Encoding.UTF8.GetBytes(plainTextPassword);

		var hasSalt = !string.IsNullOrEmpty(salt);
		if (!hasSalt)
		{	
			byte[] hashedSaltlessBytes = sha256.ComputeHash(passwordBytes);
			var saltlessHash = Convert.ToBase64String(hashedSaltlessBytes);
			return new Password(this.Type, saltlessHash);
		}
		
		byte[] saltBytes =Convert.FromBase64String(salt!);
		byte[] combinedBytes = passwordBytes.Concat(saltBytes).ToArray();
		byte[] hashedBytes = sha256.ComputeHash(combinedBytes);

		var hash = Convert.ToBase64String(hashedBytes);
		return new Password(this.Type, hash, salt!);
	}
}
