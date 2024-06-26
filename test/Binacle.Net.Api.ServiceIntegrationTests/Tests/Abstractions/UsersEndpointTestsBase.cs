﻿using Binacle.Net.Api.ServiceModule.Domain.Configuration.Models;
using Binacle.Net.Api.ServiceModule.v0.Requests;
using Binacle.Net.Api.ServiceModule.v0.Responses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Xunit;

namespace Binacle.Net.Api.ServiceIntegrationTests.Tests.Abstractions;

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

		var defaultsOptions = this.Sut.Services.GetRequiredService<IOptions<DefaultsOptions>>();
		var defaultAdminUser = defaultsOptions.Value.GetParsedAdminUser();

		this.AdminUser = new Models.TestUser
		{
			Email = defaultAdminUser.Email,
			Password = defaultAdminUser.Password
		};
	}

	protected async Task AuthenticateAsAsync(Models.TestUser user)
	{
		var request = new TokenRequest()
		{
			Email = user.Email,
			Password = user.Password
		};
		var response = await this.Sut.Client.PostAsJsonAsync("/auth/token", request, this.Sut.JsonSerializerOptions);
		var tokenResponse = await response.Content.ReadAsAsync<TokenResponse>();

		this.Sut.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);
	}

	protected async Task CreateUser(Models.TestUser user)
	{
		await this.AuthenticateAsAsync(this.AdminUser);
		var request = new CreateApiUserRequest
		{
			Email = user.Email,
			Password = user.Password
		};

		var response = await this.Sut.Client.PostAsJsonAsync("/users", request, this.Sut.JsonSerializerOptions);
		if (response.StatusCode != System.Net.HttpStatusCode.Created &&
			response.StatusCode != System.Net.HttpStatusCode.Conflict)
		{
			throw new System.ApplicationException($"Error while creating user {user.Email}. Response status code was {response.StatusCode}");
		}
	}

	protected async Task DeleteUser(Models.TestUser user)
	{
		await this.AuthenticateAsAsync(this.AdminUser);

		var response = await this.Sut.Client.DeleteAsync($"/users/{user.Email}");

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


