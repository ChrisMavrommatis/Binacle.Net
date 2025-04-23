using System.Net.Http.Json;
using Binacle.Net.ServiceModule.Application.Authentication.Services;
using Binacle.Net.ServiceModule.Application.Common.Configuration;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin;

public abstract partial class AdminEndpointsTestsBase :  IAsyncLifetime
{
	protected readonly BinacleApiAsAServiceFactory Sut;
	protected readonly Models.AccountCredentials UserAccountCredentials;
	protected readonly Models.AccountCredentials AdminAccountCredentials;
	protected readonly Domain.Accounts.Entities.Account SimulatedAdminAccount;
	
	
	public AdminEndpointsTestsBase(BinacleApiAsAServiceFactory sut)
	{
		this.Sut = sut;
		this.UserAccountCredentials = new Models.AccountCredentials
		{
			Username = "testuser@binacle.net",
			Email = "testuser@binacle.net",
			Password = "T3stUs3rsP@ssw0rd"
		};

		var options = this.Sut.Services.GetRequiredService<IOptions<ServiceModuleOptions>>();
		var defaultAdminUser = ServiceModuleOptions.ParseAccountCredentials(options.Value.DefaultAdminAccount);

		this.AdminAccountCredentials = new Models.AccountCredentials
		{
			Username = defaultAdminUser.Username,
			Email = defaultAdminUser.Email,
			Password = defaultAdminUser.Password
		};

		this.SimulatedAdminAccount = new Domain.Accounts.Entities.Account(
			defaultAdminUser.Username,
			AccountRole.Admin,
			defaultAdminUser.Email,
			AccountStatus.Active,
			new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
			Guid.Parse("6B54DFF4-8130-4572-A21A-A305F9018FFF")
		);

		var passwordHasher = this.Sut.Services.GetRequiredService<IPasswordHasher>();
		var hashedPassword = passwordHasher.CreateHash(defaultAdminUser.Password);
		this.SimulatedAdminAccount.ChangePassword(hashedPassword);
	}
	
	protected async Task AuthenticateAsAsync(Models.AccountCredentials accountCredentials)
	{
		var request = new TokenRequest()
		{
			Username = accountCredentials.Username,
			Password = accountCredentials.Password
		};
		var response = await this.Sut.Client.PostAsJsonAsync("/api/auth/token", request, this.Sut.JsonSerializerOptions);
		var tokenResponse = await response.Content.ReadAsAsync<TokenResponse>();

		this.Sut.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
	}
	
	protected async Task EnsureAccountExists(Models.AccountCredentials accountCredentials)
	{
		await this.AuthenticateAsAsync(this.AdminAccountCredentials);
		var request = new CreateAccountRequest()
		{
			Username = accountCredentials.Username,
			Email = accountCredentials.Email,
			Password = accountCredentials.Password
		};

		var response = await this.Sut.Client.PostAsJsonAsync("/api/admin/account", request, this.Sut.JsonSerializerOptions);
		if (response.StatusCode != System.Net.HttpStatusCode.Created &&
		    response.StatusCode != System.Net.HttpStatusCode.Conflict)
		{
			throw new System.ApplicationException($"Error while creating account {accountCredentials.Username}. Response status code was {response.StatusCode}");
		}

		var parts = response.Headers.Location!.LocalPath.Split(["/"], StringSplitOptions.RemoveEmptyEntries);
		var id = Guid.Parse(parts.Last());
		accountCredentials.Id = id;
	}
	
	protected async Task EnsureAccountDoesNotExist(Models.AccountCredentials accountCredentials)
	{
		await this.AuthenticateAsAsync(this.AdminAccountCredentials);

		var response = await this.Sut.Client.DeleteAsync($"/api/admin/account/{accountCredentials.Id}");

		if (response.StatusCode != System.Net.HttpStatusCode.NoContent &&
		    response.StatusCode != System.Net.HttpStatusCode.NotFound)
		{
			throw new System.ApplicationException($"Error while deleting user {accountCredentials.Username}. Response status code was {response.StatusCode}");
		}
	}
	
	public virtual async Task InitializeAsync()
	{
		await this.EnsureAccountExists(this.UserAccountCredentials);
	}

	public virtual async Task DisposeAsync()
	{
		await this.EnsureAccountDoesNotExist(this.UserAccountCredentials);
	}
}
