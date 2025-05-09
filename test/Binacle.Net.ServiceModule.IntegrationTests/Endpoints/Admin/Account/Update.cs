using Binacle.Net.ServiceModule.IntegrationTests.Models;
using System.Net.Http.Json;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.IntegrationTests.ExtensionMethods;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin.Account;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
public class Update : AdminEndpointsTestsBase
{
	private readonly AccountCredentials accountCredentialsUnderTest;

	public Update(BinacleApi sut) : base(sut)
	{
		this.accountCredentialsUnderTest = new AccountCredentials(
			Guid.Parse("3430D348-072F-4931-B2A1-51775CBA26C5"),
			"updateuser@binacle.net",
			"updateuser@binacle.net",
			"Upd4t3Us3ersP@ssw0rd"
		);
	}
	private const string routePath = "/api/admin/account/{id}";

	#region 401 Unauthorized

	[Fact(DisplayName = $"PUT {routePath}. Without Bearer Token Returns 401 Unauthorized")]
	public Task Put_WithoutBearerToken_Returns_401Unauthorized()
		=> this.Action_WithoutBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());
			var request = new AccountUpdateRequest
			{
				Username = this.accountCredentialsUnderTest.Username,
				Email = this.accountCredentialsUnderTest.Email,
				Password = this.accountCredentialsUnderTest.Password,
				Status =  AccountStatus.Active,
				Role = AccountRole.User
			};
			return await this.Sut.Client.PutAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions,	
				TestContext.Current.CancellationToken
			);
		});

	[Fact(DisplayName = $"PUT {routePath}. With Expired Bearer Token Returns 401 Unauthorized")]
	public Task Put_WithExpiredBearerToken_Returns_401Unauthorized()
		=> this.Action_WithExpiredBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());
			var request = new AccountUpdateRequest
			{
				Username = this.accountCredentialsUnderTest.Username,
				Email = this.accountCredentialsUnderTest.Email,
				Password =this.accountCredentialsUnderTest.Password,
				Status =  AccountStatus.Active,
				Role = AccountRole.User
			};
			return await this.Sut.Client.PutAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions,	
				TestContext.Current.CancellationToken
			);
		});


	[Fact(DisplayName = $"PUT {routePath}. With Wrong Issuer Bearer Token Returns 401 Unauthorized")]
	public Task Put_WithWrongIssuerBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());
			var request = new AccountUpdateRequest
			{
				Username = this.accountCredentialsUnderTest.Username,
				Email = this.accountCredentialsUnderTest.Email,
				Password =this.accountCredentialsUnderTest.Password,
				Status =  AccountStatus.Active,
				Role = AccountRole.User
			};
			return await this.Sut.Client.PutAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions,	
				TestContext.Current.CancellationToken
			);
		});

	[Fact(DisplayName = $"PUT {routePath}. With Wrong Audience Bearer Token Returns 401 Unauthorized")]
	public Task Put_WithWrongAudienceBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongAudienceBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());
			var request = new AccountUpdateRequest
			{
				Username = this.accountCredentialsUnderTest.Username,
				Email = this.accountCredentialsUnderTest.Email,
				Password =this.accountCredentialsUnderTest.Password,
				Status =  AccountStatus.Active,
				Role = AccountRole.User
			};
			return await this.Sut.Client.PutAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions,	
				TestContext.Current.CancellationToken
			);
		});

	[Fact(DisplayName = $"PUT {routePath}. With Wrongly Signed Bearer Token Returns 401 Unauthorized")]
	public Task Put_WithWronglySignedBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWronglySignedBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());
			var request = new AccountUpdateRequest
			{
				Username = this.accountCredentialsUnderTest.Username,
				Email = this.accountCredentialsUnderTest.Email,
				Password =this.accountCredentialsUnderTest.Password,
				Status =  AccountStatus.Active,
				Role = AccountRole.User
			};
			return await this.Sut.Client.PutAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions,	
				TestContext.Current.CancellationToken
			);
		});

	#endregion

	#region 403 Forbidden

	[Fact(DisplayName = $"PUT {routePath}. Without Admin Bearer Token Returns 403 Forbidden")]
	public Task Put_WithoutAdminBearerToken_Returns_403Forbidden()
		=> this.Action_WithoutAdminBearerToken_Returns_403Forbidden(async () =>
		{
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());
			var request = new AccountUpdateRequest
			{
				Username = this.accountCredentialsUnderTest.Username,
				Email = this.accountCredentialsUnderTest.Email,
				Password =this.accountCredentialsUnderTest.Password,
				Status =  AccountStatus.Active,
				Role = AccountRole.User
			};
			return await this.Sut.Client.PutAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions,	
				TestContext.Current.CancellationToken
			);
		});

	#endregion

	#region 204 No Content

	[Fact(DisplayName = $"PUT {routePath}. With Valid Request Returns 204 No Content")]
	public async Task Put_WithValidRequest_Returns_204NoContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);

		var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());
		var request = new AccountUpdateRequest
		{
			Username = this.accountCredentialsUnderTest.Username,
			Email = this.accountCredentialsUnderTest.Email,
			Password =this.accountCredentialsUnderTest.Password,
			Status =  AccountStatus.Active,
			Role = AccountRole.User
		};
		var response = await this.Sut.Client.PutAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions,	
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
	}

	#endregion

	#region 404 Not Found

	[Fact(DisplayName = $"PUT {routePath}. For Non Existing Account Returns 404 Not Found")]
	public async Task Put_ForNonExistingAccount_Returns_404NotFound()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);

		var url = routePath.Replace("{id}", this.Sut.NonExistentId.ToString());
		var request = new AccountUpdateRequest
		{
			Username = this.accountCredentialsUnderTest.Username,
			Email = this.accountCredentialsUnderTest.Email,
			Password = this.accountCredentialsUnderTest.Password,
			Status =  AccountStatus.Active,
			Role = AccountRole.User
		};

		var response = await this.Sut.Client.PutAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions,	
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
	}

	#endregion
	
	#region 422 Unprocessable Content

	[Fact(DisplayName = $"PUT {routePath}. With Invalid Email Returns 422 UnprocessableContent")]
	public async Task Put_WithInvalidEmail_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);
		var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());
		var request = new AccountUpdateRequest
		{
			Username = this.accountCredentialsUnderTest.Username,
			Email = "existinguser.test",
			Password =this.accountCredentialsUnderTest.Password,
			Status =  AccountStatus.Active,
			Role = AccountRole.User
		};
		var response = await this.Sut.Client.PutAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions,	
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.UnprocessableContent);
	}

	[Fact(DisplayName = $"PUT {routePath}. With Invalid Status Returns 422 UnprocessableContent")]
	public async Task Put_WithInvalidStatus_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);
		var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());
		var request = new AccountUpdateRequest
		{
			Username = this.accountCredentialsUnderTest.Username,
			Email = this.accountCredentialsUnderTest.Email,
			Password =this.accountCredentialsUnderTest.Password,
			Status =  null,
			Role = AccountRole.User
		};

		var response = await this.Sut.Client.PutAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions,	
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.UnprocessableContent);
	}

	[Fact(DisplayName = $"PUT {routePath}. With Invalid Role Returns 422 UnprocessableContent")]
	public async Task Put_WithInvalidRole_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);

		var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());
		var request = new AccountUpdateRequest
		{
			Username = this.accountCredentialsUnderTest.Username,
			Email = this.accountCredentialsUnderTest.Email,
			Password =this.accountCredentialsUnderTest.Password,
			Status =  AccountStatus.Active,
			Role = null
		};

		var response = await this.Sut.Client.PutAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions,	
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.UnprocessableContent);
	}
	
	[Fact(DisplayName = $"PUT {routePath}. With Invalid Password Returns 422 UnprocessableContent")]
	public async Task Put_WithInvalidPassword_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);

		var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());
		var request = new AccountUpdateRequest
		{
			Username = this.accountCredentialsUnderTest.Username,
			Email = this.accountCredentialsUnderTest.Email,
			Password = "invalid",
			Status =  AccountStatus.Active,
			Role = AccountRole.User
		};

		var response = await this.Sut.Client.PutAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions,	
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.UnprocessableContent);
	}
	
	[Fact(DisplayName = $"PUT {routePath}. With Invalid Id Returns 422 UnprocessableContent")]
	public async Task Put_WithInvalidId_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);

		var url = routePath.Replace("{id}", "invalid");

		var request = new AccountUpdateRequest
		{
			Username = this.accountCredentialsUnderTest.Username,
			Email = this.accountCredentialsUnderTest.Email,
			Password = this.accountCredentialsUnderTest.Password,
			Status =  AccountStatus.Active,
			Role = AccountRole.User
		};

		var response = await this.Sut.Client.PutAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions,	
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.UnprocessableContent);
	}

	#endregion

	public override async ValueTask InitializeAsync()
	{
		await this.Sut.EnsureAccountExists(this.accountCredentialsUnderTest);
		await base.InitializeAsync();
	}

	public override async ValueTask DisposeAsync()
	{
		await this.Sut.EnsureAccountDoesNotExist(this.accountCredentialsUnderTest);
		await base.DisposeAsync();
	}

}
