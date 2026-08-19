using System.Net;
using System.Net.Http.Json;
using Binacle.Net.ServiceModule.Domain;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.IntegrationTests.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Auth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Auth;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
public class Token : IAsyncLifetime
{
	private readonly BinacleApi sut;
	private readonly AccountCredentials suspendedAccountCredentials;
	private readonly AccountCredentials inactiveAccountCredentials;

	public Token(BinacleApi sut)
	{
		this.sut = sut;
		this.suspendedAccountCredentials = new AccountCredentials(
			Guid.Parse("3D0F0E3B-6C1A-4C0E-9E2E-7C4D5B8A1F62"),
			"suspendeduser@test.binacle.net",
			"suspendeduser@test.binacle.net",
			"Susp3nd3dUs3rP@ssw0rd"
		);
		this.inactiveAccountCredentials = new AccountCredentials(
			Guid.Parse("9B7A2C41-58E6-4D3F-8A0B-1E6F2D9C7A34"),
			"inactiveuser@test.binacle.net",
			"inactiveuser@test.binacle.net",
			"In4ct1v3Us3rP@ssw0rd"
		);
	}

	public async ValueTask InitializeAsync()
	{
		await this.sut.EnsureAccountExists(this.suspendedAccountCredentials, AccountStatus.Suspended);
		await this.sut.EnsureAccountExists(this.inactiveAccountCredentials, AccountStatus.Inactive);
	}

	public async ValueTask DisposeAsync()
	{
		await this.sut.EnsureAccountDoesNotExist(this.suspendedAccountCredentials);
		await this.sut.EnsureAccountDoesNotExist(this.inactiveAccountCredentials);
	}

	private const string routePath = "/api/auth/token";


	[Fact(DisplayName = $"POST {routePath}. With Valid Credentials Returns 200 OK")]
	public async Task Post_WithValidCredentials_Returns_200OK()
	{
		var options = this.sut.Services.GetRequiredService<IOptions<ServiceModuleOptions>>();
		var defaultAdmin = ServiceModuleOptions.ParseAccountCredentials(options.Value.DefaultAdminAccount);
		var request = new TokenRequest()
		{
			Username = defaultAdmin.Username,
			Password = defaultAdmin.Password
		};
		var response = await this.sut.Client.PostAsJsonAsync(
			routePath, 
			request,
			this.sut.JsonSerializerOptions, 
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.OK);
	}

	[Fact(DisplayName = $"POST {routePath}. With Wrong Credentials Returns 401 Unauthorized")]
	public async Task Post_WithWrongUserPassword_Returns_401Unauthorized()
	{
		var request = new TokenRequest()
		{
			Username = "validemail@test.binacle.net",
			Password = "Ag00dP@ssw0rd"
		};
		var response = await this.sut.Client.PostAsJsonAsync(
			routePath, 
			request,
			this.sut.JsonSerializerOptions, 
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
	}

	[Fact(DisplayName = $"POST {routePath}. With Invalid Credentials Returns 422 UnprocessableContent")]
	public async Task Post_WithInvalidCredentials_Returns_422UnprocessableContent()
	{
		var request = new TokenRequest()
		{
			Username = "validemail@test.binacle.net",
			Password = "pass"
		};
		var response = await this.sut.Client.PostAsJsonAsync(
			routePath, 
			request,
			this.sut.JsonSerializerOptions, 
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);
	}

	// The password is checked before the account status, so these two only pass while a suspended or inactive
	// account is given the right password. With a wrong one both collapse into the 401 above.
	[Fact(DisplayName = $"POST {routePath}. With Suspended Account Returns 403 Forbidden")]
	public async Task Post_WithSuspendedAccount_Returns_403Forbidden()
	{
		var request = new TokenRequest()
		{
			Username = this.suspendedAccountCredentials.Username,
			Password = this.suspendedAccountCredentials.Password
		};
		var response = await this.sut.Client.PostAsJsonAsync(
			routePath,
			request,
			this.sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
	}

	[Fact(DisplayName = $"POST {routePath}. With Inactive Account Returns 401 Unauthorized")]
	public async Task Post_WithInactiveAccount_Returns_401Unauthorized()
	{
		var request = new TokenRequest()
		{
			Username = this.inactiveAccountCredentials.Username,
			Password = this.inactiveAccountCredentials.Password
		};
		var response = await this.sut.Client.PostAsJsonAsync(
			routePath,
			request,
			this.sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
	}
}
