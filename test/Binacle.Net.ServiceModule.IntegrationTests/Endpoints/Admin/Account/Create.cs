using System.Net;
using System.Net.Http.Json;
using Binacle.Net.ServiceModule.IntegrationTests.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin.Account;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
[Collection(BinacleApiAsAServiceCollection.Name)]
public class Create : AdminEndpointsTestsBase
{
	private readonly AccountCredentials newAccountCredentials;

	public Create(BinacleApiAsAServiceFactory sut) : base(sut)
	{
		this.newAccountCredentials = new AccountCredentials
		{
			Username = "new@user.test",
			Email =  "new@user.test",
			Password = "N3wUs3rP@ssw0rd"
		};
	}

	private const string routePath = "/api/admin/account";

	#region 401 Unauthorized

	[Fact(DisplayName = $"POST {routePath}. Without Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithoutBearerToken_Returns_401Unauthorized()
		=> this.Action_WithoutBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new CreateAccountRequest()
			{
				Username = this.newAccountCredentials.Username,
				Email = this.newAccountCredentials.Email,
				Password = this.newAccountCredentials.Password
			};

			return await this.Sut.Client.PostAsJsonAsync(routePath, request, this.Sut.JsonSerializerOptions);
		});


	[Fact(DisplayName = $"POST {routePath}. With Expired Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithExpiredBearerToken_Returns_401Unauthorized()
		=> this.Action_WithExpiredBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new CreateAccountRequest
			{
				Username = this.newAccountCredentials.Username,
				Email = this.newAccountCredentials.Email,
				Password = this.newAccountCredentials.Password
			};

			return await this.Sut.Client.PostAsJsonAsync(routePath, request, this.Sut.JsonSerializerOptions);
		});

	[Fact(DisplayName = $"POST {routePath}. With Wrong Issuer Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithWrongIssuerBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new CreateAccountRequest
			{
				Username = this.newAccountCredentials.Username,
				Email = this.newAccountCredentials.Email,
				Password = this.newAccountCredentials.Password
			};

			return await this.Sut.Client.PostAsJsonAsync(routePath, request, this.Sut.JsonSerializerOptions);
		});


	[Fact(DisplayName = $"POST {routePath}. With Wrong Audience Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithWrongAudienceBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new CreateAccountRequest
			{
				Username = this.newAccountCredentials.Username,
				Email = this.newAccountCredentials.Email,
				Password = this.newAccountCredentials.Password
			};

			return await this.Sut.Client.PostAsJsonAsync(routePath, request, this.Sut.JsonSerializerOptions);
		});


	[Fact(DisplayName = $"POST {routePath}. With Wrongly Signed Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithWronglySignedBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWronglySignedBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new CreateAccountRequest
			{
				Username = this.newAccountCredentials.Username,
				Email = this.newAccountCredentials.Email,
				Password = this.newAccountCredentials.Password
			};

			return await this.Sut.Client.PostAsJsonAsync(routePath, request, this.Sut.JsonSerializerOptions);
		});

	#endregion

	#region 403 Forbidden

	[Fact(DisplayName = $"POST {routePath}. Without Admin User Bearer Token Returns 403 Forbidden")]
	public Task Post_WithoutAdminUserBearerToken_Returns_403Forbidden()
		=> this.Action_WithoutAdminUserBearerToken_Returns_403Forbidden(async () =>
		{
			var request = new CreateAccountRequest
			{
				Username = this.newAccountCredentials.Username,
				Email = this.newAccountCredentials.Email,
				Password = this.newAccountCredentials.Password
			};

			return await this.Sut.Client.PostAsJsonAsync(routePath, request, this.Sut.JsonSerializerOptions);
		});
	

	#endregion

	#region 201 Created

	[Fact(DisplayName = $"POST {routePath}. With Valid Credentials Returns 201 Created")]
	public async Task Post_WithValidCredentials_Returns_201Created()
	{
		await this.AuthenticateAsAsync(this.AdminAccountCredentials);

		var request = new CreateAccountRequest
		{
			Username = this.newAccountCredentials.Username,
			Email = this.newAccountCredentials.Email,
			Password = this.newAccountCredentials.Password
		};

		var response = await this.Sut.Client.PostAsJsonAsync(routePath, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(HttpStatusCode.Created);
	}
	#endregion

	#region 400 Bad Request

	[Fact(DisplayName = $"POST {routePath}. With Invalid Email Returns 400 BadRequest")]
	public async Task Post_WithInvalidEmail_Returns_400BadRequest()
	{
		await this.AuthenticateAsAsync(this.AdminAccountCredentials);

		var request = new CreateAccountRequest
		{
			Username = this.newAccountCredentials.Username,
			Email = "newuser.test",
			Password = this.newAccountCredentials.Password
		};

		var response = await this.Sut.Client.PostAsJsonAsync(routePath, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Invalid Password Returns 400 BadRequest")]
	public async Task Post_WithInvalidPassword_Returns_400BadRequest()
	{
		await this.AuthenticateAsAsync(this.AdminAccountCredentials);

		var request = new CreateAccountRequest
		{
			Username = this.newAccountCredentials.Username,
			Email = this.newAccountCredentials.Email,
			Password = "password"
		};

		var response = await this.Sut.Client.PostAsJsonAsync(routePath, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
	}
	#endregion

	#region 409 Conflict

	[Fact(DisplayName = $"POST {routePath}. For Existing User Returns 409 Conflict")]
	public async Task Post_ForExistingUser_Returns_409Conflict()
	{
		await this.AuthenticateAsAsync(this.AdminAccountCredentials);
		
		var request = new CreateAccountRequest
		{
			Username = this.UserAccountCredentials.Username,
			Email = this.UserAccountCredentials.Email,
			Password = this.UserAccountCredentials.Password
		};

		var response = await this.Sut.Client.PostAsJsonAsync(routePath, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
	}

	#endregion


	public override async Task DisposeAsync()
	{
		await this.EnsureAccountDoesNotExist(this.newAccountCredentials);
		await base.DisposeAsync();
	}
}
