using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Binacle.Net.ServiceModule.IntegrationTests.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.ServiceModule.IntegrationTests.Abstractions;

public abstract partial class UsersEndpointTestsBase : IAsyncLifetime
{
	protected readonly BinacleApiAsAServiceFactory Sut;
	protected readonly TestUser TestUser;
	protected readonly TestUser AdminUser;

	public UsersEndpointTestsBase(BinacleApiAsAServiceFactory sut)
	{
		this.Sut = sut;
		this.TestUser = new TestUser
		{
			Email = "testuser@binacle.net",
			Password = "T3stUs3rsP@ssw0rd"
		};

		var userOptions = this.Sut.Services.GetRequiredService<IOptions<UserOptions>>();
		var defaultAdminUser = userOptions.Value.GetParsedDefaultAdminUser();

		this.AdminUser = new TestUser
		{
			Email = defaultAdminUser.Email,
			Password = defaultAdminUser.Password
		};
	}

	protected async Task AuthenticateAsAsync(TestUser user)
	{
		var request = new v0.Requests.TokenRequest()
		{
			Email = user.Email,
			Password = user.Password
		};
		var response = await this.Sut.Client.PostAsJsonAsync("/api/auth/token", request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(HttpStatusCode.OK);
		var tokenResponse = await response.Content.ReadAsAsync<v0.Responses.TokenResponse>();

		this.Sut.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
	}

	protected async Task CreateUser(TestUser user)
	{
		await this.AuthenticateAsAsync(this.AdminUser);
		var request = new v0.Requests.CreateApiUserRequest
		{
			Email = user.Email,
			Password = user.Password
		};

		var response = await this.Sut.Client.PostAsJsonAsync("/api/users", request, this.Sut.JsonSerializerOptions);
		if (response.StatusCode != HttpStatusCode.Created &&
			response.StatusCode != HttpStatusCode.Conflict)
		{
			throw new ApplicationException($"Error while creating user {user.Email}. Response status code was {response.StatusCode}");
		}
	}

	protected async Task DeleteUser(TestUser user)
	{
		await this.AuthenticateAsAsync(this.AdminUser);

		var response = await this.Sut.Client.DeleteAsync($"/api/users/{user.Email}");

		if (response.StatusCode != HttpStatusCode.NoContent &&
			response.StatusCode != HttpStatusCode.NotFound)
		{
			throw new ApplicationException($"Error while deleting user {user.Email}. Response status code was {response.StatusCode}");
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


