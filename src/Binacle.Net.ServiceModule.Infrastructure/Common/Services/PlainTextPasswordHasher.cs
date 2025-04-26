using Binacle.Net.ServiceModule.Domain.Common.Models;
using Binacle.Net.ServiceModule.Domain.Common.Services;

namespace Binacle.Net.ServiceModule.Infrastructure.Common.Services;

internal class PlainTextPasswordHasher : IPasswordHasher
{
	public const string Key = "PlainText";
	public string Type => Key;

	public string? GenerateSalt()
	{
		return null;
	}
	public Password Create(string plainTextPassword, string? salt = null)
	{
		return new Password(this.Type, plainTextPassword);
	}
}
