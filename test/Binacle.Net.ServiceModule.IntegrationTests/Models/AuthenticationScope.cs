using System.Net.Http.Headers;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.ServiceModule.IntegrationTests.Models;

internal class AuthenticationScope : IAsyncDisposable, IDisposable
{
	private readonly BinacleApi sut;
	private readonly Account account;

	public AuthenticationScope(
		BinacleApi sut,
		Account account
		)
	{
		this.sut = sut;
		this.account = account;
	}

	public void Start()
	{
		var tokenService = this.sut.Services.GetRequiredService<ITokenService>();

		var token = tokenService.GenerateToken(account, null);

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
