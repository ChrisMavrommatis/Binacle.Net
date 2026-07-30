using System.Security.Cryptography;
using Binacle.Net.ServiceModule.Domain.Common.Models;
using Binacle.Net.ServiceModule.Infrastructure.Common.Services;

namespace Binacle.Net.ServiceModule.Infrastructure.Services;

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

		var hashBytes = Rfc2898DeriveBytes.Pbkdf2(
			plainTextPassword,
			saltBytes,
			_iterations, 
			HashAlgorithmName.SHA256,
			_hashSize
		);
		var hash = Convert.ToBase64String(hashBytes);
		return new Password(Type, hash, salt);
	}
}
