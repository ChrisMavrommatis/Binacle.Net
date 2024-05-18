using Binacle.Net.Api.ServiceIntegrationTests.Models;
using Binacle.Net.Api.ServiceModule.v0.Requests;
using FluentAssertions;
using System.Net.Http.Json;
using Xunit;

namespace Binacle.Net.Api.ServiceIntegrationTests.Tests;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
[Collection(BinacleApiAsAServiceCollection.Name)]
public class CreateUser : Abstractions.UsersEndpointTestsBase
{
	private readonly TestUser newUser;

	public CreateUser(BinacleApiAsAServiceFactory sut) : base(sut)
	{
		this.newUser = new TestUser
		{
			Email = "new@user.test",
			Password = "N3wUs3rP@ssw0rd"
		};
	}

	private const string routePath = "/users";

	#region 401 Unauthorized

	[Fact(DisplayName = $"POST {routePath}. Without Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithoutBearerToken_Returns_401Unauthorized()
		=> this.Action_WithoutBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new CreateApiUserRequest
			{
				Email = this.newUser.Email,
				Password = this.newUser.Password
			};

			return await this.Sut.Client.PostAsJsonAsync(routePath, request, this.Sut.JsonSerializerOptions);
		});


	[Fact(DisplayName = $"POST {routePath}. With Expired Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithExpiredBearerToken_Returns_401Unauthorized()
		=> this.Action_WithExpiredBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new CreateApiUserRequest
			{
				Email = this.newUser.Email,
				Password = this.newUser.Password
			};

			return await this.Sut.Client.PostAsJsonAsync(routePath, request, this.Sut.JsonSerializerOptions);
		});

	[Fact(DisplayName = $"POST {routePath}. With Wrong Issuer Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithWrongIssuerBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new CreateApiUserRequest
			{
				Email = this.newUser.Email,
				Password = this.newUser.Password
			};

			return await this.Sut.Client.PostAsJsonAsync(routePath, request, this.Sut.JsonSerializerOptions);
		});


	[Fact(DisplayName = $"POST {routePath}. With Wrong Audience Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithWrongAudienceBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new CreateApiUserRequest
			{
				Email = this.newUser.Email,
				Password = this.newUser.Password
			};

			return await this.Sut.Client.PostAsJsonAsync(routePath, request, this.Sut.JsonSerializerOptions);
		});


	[Fact(DisplayName = $"POST {routePath}. With Wrongly Signed Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithWronglySignedBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWronglySignedBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new CreateApiUserRequest
			{
				Email = this.newUser.Email,
				Password = this.newUser.Password
			};

			return await this.Sut.Client.PostAsJsonAsync(routePath, request, this.Sut.JsonSerializerOptions);
		});

	#endregion

	#region 403 Forbidden

	[Fact(DisplayName = $"POST {routePath}. Without Admin User Bearer Token Returns 403 Forbidden")]
	public Task Post_WithoutAdminUserBearerToken_Returns_403Forbidden()
		=> this.Action_WithoutAdminUserBearerToken_Returns_403Forbidden(async () =>
		{
			var request = new CreateApiUserRequest
			{
				Email = this.newUser.Email,
				Password = this.newUser.Password
			};

			return await this.Sut.Client.PostAsJsonAsync(routePath, request, this.Sut.JsonSerializerOptions);
		});
	

	#endregion

	#region 201 Created

	[Fact(DisplayName = $"POST {routePath}. With Valid Credentials Returns 201 Created")]
	public async Task Post_WithValidCredentials_Returns_201Created()
	{
		await this.AuthenticateAsAsync(this.AdminUser);

		var request = new CreateApiUserRequest
		{
			Email = this.newUser.Email,
			Password = this.newUser.Password
		};

		var response = await this.Sut.Client.PostAsJsonAsync(routePath, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
	}
	#endregion

	#region 400 Bad Request

	[Fact(DisplayName = $"POST {routePath}. With Invalid Email Returns 400 BadRequest")]
	public async Task Post_WithInvalidEmail_Returns_400BadRequest()
	{
		await this.AuthenticateAsAsync(this.AdminUser);

		var request = new CreateApiUserRequest
		{
			Email = "newuser.test",
			Password = this.newUser.Password
		};

		var response = await this.Sut.Client.PostAsJsonAsync(routePath, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Invalid Password Returns 400 BadRequest")]
	public async Task Post_WithInvalidPassword_Returns_400BadRequest()
	{
		await this.AuthenticateAsAsync(this.AdminUser);

		var request = new CreateApiUserRequest
		{
			Email = this.newUser.Email,
			Password = "password"
		};

		var response = await this.Sut.Client.PostAsJsonAsync(routePath, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
	}
	#endregion

	#region 409 Conflict

	[Fact(DisplayName = $"POST {routePath}. For Existing User Returns 409 Conflict")]
	public async Task Post_ForExistingUser_Returns_409Conflict()
	{
		await this.AuthenticateAsAsync(this.AdminUser);

		var request = new CreateApiUserRequest
		{
			Email = this.TestUser.Email,
			Password = this.TestUser.Password
		};

		var response = await this.Sut.Client.PostAsJsonAsync(routePath, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
	}

	#endregion


	public override async Task DisposeAsync()
	{
		await this.DeleteUser(this.newUser);
		await base.DisposeAsync();
	}
}
