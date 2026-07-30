using Binacle.Net.ServiceModule.Domain.Common.Models;

namespace Binacle.Net.ServiceModule.Infrastructure.Common.Services;

internal interface IPasswordHasher
{
	string Type { get; }

	string? GenerateSalt();

	Password Create(string plainTextPassword, string? salt = null);
}
