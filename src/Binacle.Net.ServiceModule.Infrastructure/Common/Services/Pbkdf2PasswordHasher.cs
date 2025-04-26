using System.Security.Cryptography;
using Binacle.Net.ServiceModule.Domain.Common.Models;
using Binacle.Net.ServiceModule.Domain.Common.Services;

namespace Binacle.Net.ServiceModule.Infrastructure.Common.Services;

internal class Pbkdf2PasswordHasher : IPasswordHasher
{
	public const string Key = "PBKDF2";
	
	public string Type => Key;
	
	private const int _iterations = 100_000;
	private const int _saltSize = 16; // 128 Bits
	private const int _hashSize = 32; // 256 Bits

	public string? GenerateSalt()
	{
		return Convert.ToBase64String(RandomNumberGenerator.GetBytes(_saltSize));
	}

	public Password Create(string plainTextPassword, string? salt = null)
	{
		if (string.IsNullOrEmpty(salt))
			salt = this.GenerateSalt();

		var saltBytes = Convert.FromBase64String(salt!);

		using var pbkdf2 = new Rfc2898DeriveBytes(plainTextPassword, saltBytes, _iterations, HashAlgorithmName.SHA256);
		var hash = Convert.ToBase64String(pbkdf2.GetBytes(_hashSize));
		return new Password(Type, hash, salt);
	}
}
