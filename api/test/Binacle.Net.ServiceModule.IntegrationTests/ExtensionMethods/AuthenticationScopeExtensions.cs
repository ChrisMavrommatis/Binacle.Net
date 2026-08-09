using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.IntegrationTests.Models;

namespace Binacle.Net.ServiceModule.IntegrationTests.ExtensionMethods;

internal static class AuthenticationScopeExtensions
{
	public static AuthenticationScope StartAuthenticationScope(this BinacleApi sut, HttpClient client, Account account)
	{
		var scope = new AuthenticationScope(sut, client, account);
		scope.Start();
		return scope;
	}
}
