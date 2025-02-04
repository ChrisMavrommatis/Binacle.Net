using Binacle.Net.Api.ServiceModule.Domain.Configuration.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Binacle.Net.Api.ServiceModule.IntegrationTests.Abstractions;

public abstract partial class UsersEndpointTestsBase : IAsyncLifetime
{
	protected readonly BinacleApiAsAServiceFactory Sut;
	protected readonly Models.TestUser TestUser;
	protected readonly Models.TestUser AdminUser;

	public UsersEndpointTestsBase(BinacleApiAsAServiceFactory sut)
	{
		this.Sut = sut;
		this.TestUser = new Models.TestUser
		{
			Email = "testuser@binacle.net",
			Password = "T3stUs3rsP@ssw0rd"
		};

		var userOptions = this.Sut.Services.GetRequiredService<IOptions<UserOptions>>();
		var defaultAdminUser = userOptions.Value.GetParsedDefaultAdminUser();

		this.AdminUser = new Models.TestUser
		{
			Email = defaultAdminUser.Email,
			Password = defaultAdminUser.Password
		};
	}

	protected async Task AuthenticateAsAsync(Models.TestUser user)
	{
		var request = new ServiceModule.v0.Requests.TokenRequest()
		{
			Email = user.Email,
			Password = user.Password
		};
		var response = await this.Sut.Client.PostAsJsonAsync("/api/auth/token", request, this.Sut.JsonSerializerOptions);
		var tokenResponse = await response.Content.ReadAsAsync<ServiceModule.v0.Responses.TokenResponse>();

		this.Sut.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
	}

	protected async Task CreateUser(Models.TestUser user)
	{
		await this.AuthenticateAsAsync(this.AdminUser);
		var request = new ServiceModule.v0.Requests.CreateApiUserRequest
		{
			Email = user.Email,
			Password = user.Password
		};

		var response = await this.Sut.Client.PostAsJsonAsync("/api/users", request, this.Sut.JsonSerializerOptions);
		if (response.StatusCode != System.Net.HttpStatusCode.Created &&
			response.StatusCode != System.Net.HttpStatusCode.Conflict)
		{
			throw new System.ApplicationException($"Error while creating user {user.Email}. Response status code was {response.StatusCode}");
		}
	}

	protected async Task DeleteUser(Models.TestUser user)
	{
		await this.AuthenticateAsAsync(this.AdminUser);

		var response = await this.Sut.Client.DeleteAsync($"/api/users/{user.Email}");

		if (response.StatusCode != System.Net.HttpStatusCode.NoContent &&
			response.StatusCode != System.Net.HttpStatusCode.NotFound)
		{
			throw new System.ApplicationException($"Error while deleting user {user.Email}. Response status code was {response.StatusCode}");
		}
	}

	public virtual async Task InitializeAsync()
	{
		await this.CreateUser(this.TestUser);
	}

	public virtual async Task DisposeAsync()
	{
		await this.DeleteUser(this.TestUser);
	}
}


