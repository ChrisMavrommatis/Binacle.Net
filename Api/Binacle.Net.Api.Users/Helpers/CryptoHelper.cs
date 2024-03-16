using System.Security.Cryptography;
using System.Text;

namespace Binacle.Net.Api.Users.Helpers;

internal static class CryptoHelper
{
	internal static byte[] GenerateSalt()
	{
		return RandomNumberGenerator.GetBytes(16);
	}

	internal static string HashPassword(string password, byte[] salt)
	{
		using (var sha256 = SHA256.Create())
		{
			byte[] combinedBytes = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();
			byte[] hashedBytes = sha256.ComputeHash(combinedBytes);
			return Convert.ToBase64String(hashedBytes);
		}
	}
}
