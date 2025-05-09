using System.Net;
using System.Net.Http.Json;
using Binacle.Net.ServiceModule.IntegrationTests.ExtensionMethods;
using Binacle.Net.ServiceModule.IntegrationTests.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin.Account;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
public class Create : AdminEndpointsTestsBase
{
	private readonly AccountCredentials accountCredentialsUnderTest;
	
	public Create(BinacleApi sut) : base(sut)
	{
		this.accountCredentialsUnderTest = new AccountCredentials(
			Guid.Parse("C7A945CD-ECA7-4FA8-BCAC-46D678389905"),
			"createuser@binacle.net",
			"createuser@binacle.net",
			"cr3AtEUs3ersP@ssw0rd"
		);
	}

	private const string routePath = "/api/admin/account";

	#region 401 Unauthorized

	[Fact(DisplayName = $"POST {routePath}. Without Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithoutBearerToken_Returns_401Unauthorized()
		=> this.Action_WithoutBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new AccountCreateRequest()
			{
				Username = this.accountCredentialsUnderTest.Username,
				Email = this.accountCredentialsUnderTest.Email,
				Password = this.accountCredentialsUnderTest.Password
			};

			return await this.Sut.Client.PostAsJsonAsync(
				routePath, 
				request,
				this.Sut.JsonSerializerOptions,
				TestContext.Current.CancellationToken
			);
		});


	[Fact(DisplayName = $"POST {routePath}. With Expired Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithExpiredBearerToken_Returns_401Unauthorized()
		=> this.Action_WithExpiredBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new AccountCreateRequest
			{
				Username = this.accountCredentialsUnderTest.Username,
				Email = this.accountCredentialsUnderTest.Email,
				Password = this.accountCredentialsUnderTest.Password
			};

			return await this.Sut.Client.PostAsJsonAsync(
				routePath, 
				request,
				this.Sut.JsonSerializerOptions,
				TestContext.Current.CancellationToken
			);
		});

	[Fact(DisplayName = $"POST {routePath}. With Wrong Issuer Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithWrongIssuerBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new AccountCreateRequest
			{
				Username = this.accountCredentialsUnderTest.Username,
				Email = this.accountCredentialsUnderTest.Email,
				Password = this.accountCredentialsUnderTest.Password
			};

			return await this.Sut.Client.PostAsJsonAsync(
				routePath, 
				request,
				this.Sut.JsonSerializerOptions,
				TestContext.Current.CancellationToken
			);
		});


	[Fact(DisplayName = $"POST {routePath}. With Wrong Audience Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithWrongAudienceBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new AccountCreateRequest
			{
				Username = this.accountCredentialsUnderTest.Username,
				Email = this.accountCredentialsUnderTest.Email,
				Password = this.accountCredentialsUnderTest.Password
			};

			return await this.Sut.Client.PostAsJsonAsync(
				routePath, 
				request,
				this.Sut.JsonSerializerOptions,
				TestContext.Current.CancellationToken
			);
		});


	[Fact(DisplayName = $"POST {routePath}. With Wrongly Signed Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithWronglySignedBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWronglySignedBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new AccountCreateRequest
			{
				Username = this.accountCredentialsUnderTest.Username,
				Email = this.accountCredentialsUnderTest.Email,
				Password = this.accountCredentialsUnderTest.Password
			};

			return await this.Sut.Client.PostAsJsonAsync(
				routePath, 
				request,
				this.Sut.JsonSerializerOptions,
				TestContext.Current.CancellationToken
			);
		});

	#endregion

	#region 403 Forbidden

	[Fact(DisplayName = $"POST {routePath}. Without Admin Bearer Token Returns 403 Forbidden")]
	public Task Post_WithoutAdminBearerToken_Returns_403Forbidden()
		=> this.Action_WithoutAdminBearerToken_Returns_403Forbidden(async () =>
		{
			var request = new AccountCreateRequest
			{
				Username = this.accountCredentialsUnderTest.Username,
				Email = this.accountCredentialsUnderTest.Email,
				Password = this.accountCredentialsUnderTest.Password
			};

			return await this.Sut.Client.PostAsJsonAsync(
				routePath, 
				request,
				this.Sut.JsonSerializerOptions,
				TestContext.Current.CancellationToken
			);
		});

	#endregion

	#region 201 Created

	[Fact(DisplayName = $"POST {routePath}. With Valid Request Returns 201 Created")]
	public async Task Post_WithValidRequest_Returns_201Created()
	{
		await this.Sut.EnsureAccountDoesNotExist(this.accountCredentialsUnderTest);
		
		await using (var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin))
		{
			var request = new AccountCreateRequest
			{
				Username = this.accountCredentialsUnderTest.Username,
				Email = this.accountCredentialsUnderTest.Email,
				Password = this.accountCredentialsUnderTest.Password
			};

			var response = 	await this.Sut.Client.PostAsJsonAsync(
				routePath, 
				request,
				this.Sut.JsonSerializerOptions,
				TestContext.Current.CancellationToken
			);
			response.StatusCode.ShouldBe(HttpStatusCode.Created);
		}

		await this.Sut.EnsureAccountDoesNotExist(this.accountCredentialsUnderTest);
	}

	#endregion

	#region 409 Conflict

	[Fact(DisplayName = $"POST {routePath}. For Existing Account Returns 409 Conflict")]
	public async Task Post_ForExistingAccount_Returns_409Conflict()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);

		var request = new AccountCreateRequest
		{
			Username = this.Sut.ExistingAccountCredentials.Username,
			Email = this.Sut.ExistingAccountCredentials.Email,
			Password = this.Sut.ExistingAccountCredentials.Password
		};

		var response = await this.Sut.Client.PostAsJsonAsync(
			routePath,
			request,
			this.Sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
	}

	#endregion

	#region 422 Unprocessable Content

	[Fact(DisplayName = $"POST {routePath}. With Invalid Email Returns 422 UnprocessableContent")]
	public async Task Post_WithInvalidEmail_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);

		var request = new AccountCreateRequest
		{
			Username = this.accountCredentialsUnderTest.Username,
			Email = "newuser.test",
			Password = this.accountCredentialsUnderTest.Password
		};

		var response = await this.Sut.Client.PostAsJsonAsync(
			routePath,
			request,
			this.Sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);
	}

	[Fact(DisplayName = $"POST {routePath}. With Invalid Password Returns 422 UnprocessableContent")]
	public async Task Post_WithInvalidPassword_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);

		var request = new AccountCreateRequest
		{
			Username = this.accountCredentialsUnderTest.Username,
			Email = this.accountCredentialsUnderTest.Email,
			Password = "password"
		};

		var response = await this.Sut.Client.PostAsJsonAsync(
			routePath,
			request,
			this.Sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);
	}

	#endregion

	public override async ValueTask InitializeAsync()
	{
		await base.InitializeAsync();
	}

	public override async ValueTask DisposeAsync()
	{
		await base.DisposeAsync();
	}
}
