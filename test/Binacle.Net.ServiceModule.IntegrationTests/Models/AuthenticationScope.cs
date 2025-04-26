using System.Net.Http.Headers;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Models;
using Binacle.Net.ServiceModule.Services;
using FluxResults.Unions;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.ServiceModule.IntegrationTests.Models;

internal class AuthenticationScope : IAsyncDisposable, IDisposable
{
	private readonly BinacleApiAsAServiceFactory sut;
	private readonly Account account;

	public AuthenticationScope(
		BinacleApiAsAServiceFactory sut,
		Account account
		)
	{
		this.sut = sut;
		this.account = account;
	}

	public void Start()
	{
		var tokenService = this.sut.Services.GetRequiredService<ITokenService>();

		var result = tokenService.GenerateToken(account, null);
		var token = result.Unwrap<Token>();

		sut.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenValue);
	}
	
	public void Dispose()
	{
		this.sut.Client.DefaultRequestHeaders.Authorization = null;
	}

	public ValueTask DisposeAsync()
	{
		this.sut.Client.DefaultRequestHeaders.Authorization = null;
		return ValueTask.CompletedTask;
	}
}
