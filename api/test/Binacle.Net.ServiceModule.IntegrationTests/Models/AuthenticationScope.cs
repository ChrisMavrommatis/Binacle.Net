using System.Net.Http.Headers;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.ServiceModule.IntegrationTests.Models;

internal class AuthenticationScope : IAsyncDisposable, IDisposable
{
	private readonly BinacleApi sut;
	private readonly HttpClient client;
	private readonly Account account;

	public AuthenticationScope(
		BinacleApi sut,
		HttpClient client,
		Account account
		)
	{
		this.sut = sut;
		this.client = client;
		this.account = account;
	}

	public void Start()
	{
		var tokenService = this.sut.Services.GetRequiredService<ITokenService>();

		var token = tokenService.GenerateToken(account, null);

		this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.TokenValue);
	}

	public void Dispose()
	{
		this.client.DefaultRequestHeaders.Authorization = null;
	}

	public ValueTask DisposeAsync()
	{
		this.client.DefaultRequestHeaders.Authorization = null;
		return ValueTask.CompletedTask;
	}
}
