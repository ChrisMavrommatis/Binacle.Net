using Binacle.Net.ServiceModule.IntegrationTests.Models;
using System.Net.Http.Json;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.IntegrationTests.ExtensionMethods;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin.Account;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
[Collection(BinacleApiAsAServiceCollection.Name)]
public class Update : AdminEndpointsTestsBase
{
	private readonly AccountCredentials existingAccountCredentials;

	public Update(BinacleApiAsAServiceFactory sut) : base(sut)
	{
		this.existingAccountCredentials = new AccountCredentials()
		{
			Username =  "existinguser@binacle.net",
			Email = "existinguser@binacle.net",
			Password = "Ex1stingUs3rP@ssw0rd"

		};
	}
	private const string routePath = "/api/admin/account/{id}";

	#region 401 Unauthorized

	[Fact(DisplayName = $"PUT {routePath}. Without Bearer Token Returns 401 Unauthorized")]
	public Task Put_WithoutBearerToken_Returns_401Unauthorized()
		=> this.Action_WithoutBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			var request = new UpdateAccountRequest
			{
				Username = this.existingAccountCredentials.Username,
				Email = this.existingAccountCredentials.Email,
				Password =this.existingAccountCredentials.Password,
				Status =  AccountStatus.Active,
				Role = AccountRole.User
			};
			return await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});

	[Fact(DisplayName = $"PUT {routePath}. With Expired Bearer Token Returns 401 Unauthorized")]
	public Task Put_WithExpiredBearerToken_Returns_401Unauthorized()
		=> this.Action_WithExpiredBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			var request = new UpdateAccountRequest
			{
				Username = this.existingAccountCredentials.Username,
				Email = this.existingAccountCredentials.Email,
				Password =this.existingAccountCredentials.Password,
				Status =  AccountStatus.Active,
				Role = AccountRole.User
			};
			return await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});


	[Fact(DisplayName = $"PUT {routePath}. With Wrong Issuer Bearer Token Returns 401 Unauthorized")]
	public Task Put_WithWrongIssuerBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			var request = new UpdateAccountRequest
			{
				Username = this.existingAccountCredentials.Username,
				Email = this.existingAccountCredentials.Email,
				Password =this.existingAccountCredentials.Password,
				Status =  AccountStatus.Active,
				Role = AccountRole.User
			};
			return await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});

	[Fact(DisplayName = $"PUT {routePath}. With Wrong Audience Bearer Token Returns 401 Unauthorized")]
	public Task Put_WithWrongAudienceBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongAudienceBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			var request = new UpdateAccountRequest
			{
				Username = this.existingAccountCredentials.Username,
				Email = this.existingAccountCredentials.Email,
				Password =this.existingAccountCredentials.Password,
				Status =  AccountStatus.Active,
				Role = AccountRole.User
			};
			return await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});

	[Fact(DisplayName = $"PUT {routePath}. With Wrongly Signed Bearer Token Returns 401 Unauthorized")]
	public Task Put_WithWronglySignedBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWronglySignedBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			var request = new UpdateAccountRequest
			{
				Username = this.existingAccountCredentials.Username,
				Email = this.existingAccountCredentials.Email,
				Password =this.existingAccountCredentials.Password,
				Status =  AccountStatus.Active,
				Role = AccountRole.User
			};
			return await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});

	#endregion

	#region 403 Forbidden

	[Fact(DisplayName = $"PUT {routePath}. Without Admin Bearer Token Returns 403 Forbidden")]
	public Task Put_WithoutAdminBearerToken_Returns_403Forbidden()
		=> this.Action_WithoutAdminBearerToken_Returns_403Forbidden(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			var request = new UpdateAccountRequest
			{
				Username = this.existingAccountCredentials.Username,
				Email = this.existingAccountCredentials.Email,
				Password =this.existingAccountCredentials.Password,
				Status =  AccountStatus.Active,
				Role = AccountRole.User
			};
			return await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});

	#endregion

	#region 204 No Content

	[Fact(DisplayName = $"PUT {routePath}. With Valid Request Returns 204 No Content")]
	public async Task Put_WithValidRequest_Returns_204NoContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);

		var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
		var request = new UpdateAccountRequest
		{
			Username = this.existingAccountCredentials.Username,
			Email = this.existingAccountCredentials.Email,
			Password =this.existingAccountCredentials.Password,
			Status =  AccountStatus.Active,
			Role = AccountRole.User
		};
		var response = await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
	}

	
	#endregion

	#region 400 Bad Request

	[Fact(DisplayName = $"PUT {routePath}. With Invalid Email Returns 400 BadRequest")]
	public async Task Put_WithInvalidEmail_Returns_400BadRequest()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);
		var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
		var request = new UpdateAccountRequest
		{
			Username = this.existingAccountCredentials.Username,
			Email = "existinguser.test",
			Password =this.existingAccountCredentials.Password,
			Status =  AccountStatus.Active,
			Role = AccountRole.User
		};
		var response = await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
	}

	[Fact(DisplayName = $"PUT {routePath}. With Invalid Status Returns 400 BadRequest")]
	public async Task Put_WithInvalidStatus_Returns_400BadRequest()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);
		var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
		var request = new UpdateAccountRequest
		{
			Username = this.existingAccountCredentials.Username,
			Email = this.existingAccountCredentials.Email,
			Password =this.existingAccountCredentials.Password,
			Status =  null,
			Role = AccountRole.User
		};

		var response = await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
	}

	[Fact(DisplayName = $"PUT {routePath}. With Invalid Role Returns 400 BadRequest")]
	public async Task Put_WithInvalidRole_Returns_400BadRequest()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);

		var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
		var request = new UpdateAccountRequest
		{
			Username = this.existingAccountCredentials.Username,
			Email = this.existingAccountCredentials.Email,
			Password =this.existingAccountCredentials.Password,
			Status =  AccountStatus.Active,
			Role = null
		};

		var response = await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
	}
	
	[Fact(DisplayName = $"PUT {routePath}. With Invalid Password Returns 400 BadRequest")]
	public async Task Put_WithInvalidPassword_Returns_400BadRequest()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);

		var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
		var request = new UpdateAccountRequest
		{
			Username = this.existingAccountCredentials.Username,
			Email = this.existingAccountCredentials.Email,
			Password = "invalid",
			Status =  AccountStatus.Active,
			Role = AccountRole.User
		};

		var response = await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
	}

	#endregion

	#region 404 Not Found

	[Fact(DisplayName = $"PUT {routePath}. For Non Existing User Returns 404 Not Found")]
	public async Task Put_ForNonExistingUser_Returns_404NotFound()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);
		var nonExistentId = Guid.Parse("EF81C267-A003-44B8-AD89-4B48661C4AA5");

		var url = routePath.Replace("{id}", nonExistentId.ToString());
		var request = new UpdateAccountRequest
		{
			Username = this.existingAccountCredentials.Username,
			Email = this.existingAccountCredentials.Email,
			Password = this.existingAccountCredentials.Password,
			Status =  AccountStatus.Active,
			Role = AccountRole.User
		};

		var response = await this.Sut.Client.PutAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
	}

	#endregion

	public override async Task InitializeAsync()
	{
		await this.EnsureAccountExists(this.existingAccountCredentials);
		await base.InitializeAsync();
	}

	public override async Task DisposeAsync()
	{
		await this.EnsureAccountExists(this.existingAccountCredentials);
		await base.DisposeAsync();
	}

}
