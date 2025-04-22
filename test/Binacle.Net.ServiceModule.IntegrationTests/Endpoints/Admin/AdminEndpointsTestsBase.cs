using System.Net.Http.Json;
using Binacle.Net.ServiceModule.Application.Common.Configuration;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin;

public abstract partial class AdminEndpointsTestsBase :  IAsyncLifetime
{
	protected readonly BinacleApiAsAServiceFactory Sut;
	protected readonly Models.TestAccount SimpleAccount;
	protected readonly Models.TestAccount AdminAccount;
	
	
	public AdminEndpointsTestsBase(BinacleApiAsAServiceFactory sut)
	{
		this.Sut = sut;
		this.SimpleAccount = new Models.TestAccount
		{
			Username = "testuser@binacle.net",
			Password = "T3stUs3rsP@ssw0rd"
		};

		var options = this.Sut.Services.GetRequiredService<IOptions<ServiceModuleOptions>>();
		var defaultAdminUser = ServiceModuleOptions.ParseAccountCredentials(options.Value.DefaultAdminAccount);

		this.AdminAccount = new Models.TestAccount
		{
			Username = defaultAdminUser.Username,
			Password = defaultAdminUser.Password
		};
	}
	
	protected async Task AuthenticateAsAsync(Models.TestAccount account)
	{
		var request = new TokenRequest()
		{
			Username = account.Username,
			Password = account.Password
		};
		var response = await this.Sut.Client.PostAsJsonAsync("/api/auth/token", request, this.Sut.JsonSerializerOptions);
		var tokenResponse = await response.Content.ReadAsAsync<TokenResponse>();

		this.Sut.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
	}
	
	protected async Task EnsureAccountExists(Models.TestAccount account)
	{
		await this.AuthenticateAsAsync(this.AdminAccount);
		var request = new CreateAccountRequest()
		{
			Username = account.Username,
			Email = account.Username,
			Password = account.Password
		};

		var response = await this.Sut.Client.PostAsJsonAsync("/api/admin/account", request, this.Sut.JsonSerializerOptions);
		if (response.StatusCode != System.Net.HttpStatusCode.Created &&
		    response.StatusCode != System.Net.HttpStatusCode.Conflict)
		{
			throw new System.ApplicationException($"Error while creating account {account.Username}. Response status code was {response.StatusCode}");
		}
	}
	
	protected async Task EnsureAccountDoesNotExist(Models.TestAccount account)
	{
		await this.AuthenticateAsAsync(this.AdminAccount);

		var response = await this.Sut.Client.DeleteAsync($"/api/admin/a{user.Email}");

		if (response.StatusCode != System.Net.HttpStatusCode.NoContent &&
		    response.StatusCode != System.Net.HttpStatusCode.NotFound)
		{
			throw new System.ApplicationException($"Error while deleting user {user.Email}. Response status code was {response.StatusCode}");
		}
	}
	
	public virtual async Task InitializeAsync()
	{
		await this.EnsureAccountExists(this.SimpleAccount);
	}

	public virtual async Task DisposeAsync()
	{
		await this.DeleteUser(this.TestUser);
	}
}
