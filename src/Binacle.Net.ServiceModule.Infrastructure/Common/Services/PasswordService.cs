using Binacle.Net.ServiceModule.Domain.Common.Models;
using Binacle.Net.ServiceModule.Domain.Common.Services;

namespace Binacle.Net.ServiceModule.Infrastructure.Common.Services;

internal class PasswordService : IPasswordService
{
	private static string _default = Pbkdf2PasswordHasher.Key;
	private readonly Dictionary<string, IPasswordHasher> hashers;

	public PasswordService(
		IEnumerable<IPasswordHasher> hashers
	)
	{
		this.hashers = hashers.ToDictionary(x => x.Type, StringComparer.OrdinalIgnoreCase);
	}

	public Password Create(string plainTextPassword, string? type = null)
	{
		var hasherType = type ?? _default;
		if (!this.hashers.TryGetValue(hasherType, out var hasher))
		{
			throw new InvalidOperationException($"No hasher registered for type '{hasherType}'");
		}

		var salt = hasher.GenerateSalt();
		return hasher.Create(plainTextPassword, salt);
	}

	public bool PasswordMatches(Password storedPassword, string plainTextPassword)
	{
		if (!this.hashers.TryGetValue(storedPassword.Type, out var hasher))
			throw new InvalidOperationException($"No hasher registered for type '{storedPassword.Type}'");

		var newPassword = hasher.Create(plainTextPassword, storedPassword.Salt);
		return storedPassword.Equals(newPassword);
	}
}
