using Binacle.Net.ServiceModule.IntegrationTests.Models;
using System.Net.Http.Json;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.IntegrationTests.ExtensionMethods;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin.Account;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
[Collection(BinacleApiAsAServiceCollection.Name)]
public class Patch : AdminEndpointsTestsBase
{
	private readonly AccountCredentials existingAccountCredentials;

	public Patch(BinacleApiAsAServiceFactory sut) : base(sut)
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

	[Fact(DisplayName = $"PATCH {routePath}. Without Bearer Token Returns 401 Unauthorized")]
	public Task Patch_WithoutBearerToken_Returns_401Unauthorized()
		=> this.Action_WithoutBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			var request = new AccountPatchRequest
			{
				Username = this.existingAccountCredentials.Username,
				Email = this.existingAccountCredentials.Email,
				Password = this.existingAccountCredentials.Password,
				Status =  AccountStatus.Active,
				Role = AccountRole.User
			};
			return await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});

	[Fact(DisplayName = $"PATCH {routePath}. With Expired Bearer Token Returns 401 Unauthorized")]
	public Task Patch_WithExpiredBearerToken_Returns_401Unauthorized()
		=> this.Action_WithExpiredBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			var request = new AccountPatchRequest
			{
				Username = this.existingAccountCredentials.Username,
				Email = this.existingAccountCredentials.Email,
				Password =this.existingAccountCredentials.Password,
				Status =  AccountStatus.Active,
				Role = AccountRole.User
			};
			return await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});


	[Fact(DisplayName = $"PATCH {routePath}. With Wrong Issuer Bearer Token Returns 401 Unauthorized")]
	public Task Patch_WithWrongIssuerBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			var request = new AccountPatchRequest
			{
				Username = this.existingAccountCredentials.Username,
				Email = this.existingAccountCredentials.Email,
				Password =this.existingAccountCredentials.Password,
				Status =  AccountStatus.Active,
				Role = AccountRole.User
			};
			return await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});

	[Fact(DisplayName = $"PATCH {routePath}. With Wrong Audience Bearer Token Returns 401 Unauthorized")]
	public Task Patch_WithWrongAudienceBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongAudienceBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			var request = new AccountPatchRequest
			{
				Username = this.existingAccountCredentials.Username,
				Email = this.existingAccountCredentials.Email,
				Password =this.existingAccountCredentials.Password,
				Status =  AccountStatus.Active,
				Role = AccountRole.User
			};
			return await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});

	[Fact(DisplayName = $"PATCH {routePath}. With Wrongly Signed Bearer Token Returns 401 Unauthorized")]
	public Task Patch_WithWronglySignedBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWronglySignedBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			var request = new AccountPatchRequest
			{
				Username = this.existingAccountCredentials.Username,
				Email = this.existingAccountCredentials.Email,
				Password =this.existingAccountCredentials.Password,
				Status =  AccountStatus.Active,
				Role = AccountRole.User
			};
			return await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});

	#endregion

	#region 403 Forbidden

	[Fact(DisplayName = $"PATCH {routePath}. Without Admin Bearer Token Returns 403 Forbidden")]
	public Task Patch_WithoutAdminBearerToken_Returns_403Forbidden()
		=> this.Action_WithoutAdminBearerToken_Returns_403Forbidden(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			var request = new AccountPatchRequest
			{
				Username = this.existingAccountCredentials.Username,
				Email = this.existingAccountCredentials.Email,
				Password =this.existingAccountCredentials.Password,
				Status =  AccountStatus.Active,
				Role = AccountRole.User
			};
			return await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});

	#endregion

	#region 204 No Content

	[Fact(DisplayName = $"PATCH {routePath}. With Full Valid Request Returns 204 No Content")]
	public async Task Patch_WithFullValidRequest_Returns_204NoContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);

		var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
		var request = new AccountPatchRequest
		{
			Username = this.existingAccountCredentials.Username,
			Email = this.existingAccountCredentials.Email,
			Password =this.existingAccountCredentials.Password,
			Status =  AccountStatus.Active,
			Role = AccountRole.User
		};
		var response = await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
	}
	
	[Fact(DisplayName = $"PATCH {routePath}. With Partial Valid Request Returns 204 No Content")]
	public async Task Patch_WithPartialValidequest_Returns_204NoContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);

		var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
		var request = new AccountPatchRequest
		{
			Status =  AccountStatus.Active,
			Role = AccountRole.User
		};
		var response = await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
	}

	
	#endregion

	#region 404 Not Found

	[Fact(DisplayName = $"PATCH {routePath}. For Non Existing Account Returns 404 Not Found")]
	public async Task  Patch_ForNonExistingAccount_Returns_404NotFound()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);
		var nonExistentId = Guid.Parse("EF81C267-A003-44B8-AD89-4B48661C4AA5");

		var url = routePath.Replace("{id}", nonExistentId.ToString());
		var request = new AccountPatchRequest
		{
			Username = this.existingAccountCredentials.Username,
			Email = this.existingAccountCredentials.Email,
			Password = this.existingAccountCredentials.Password,
			Status =  AccountStatus.Active,
			Role = AccountRole.User
		};

		var response = await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
	}

	#endregion
	
	#region 422 Unprocessable Content

	[Fact(DisplayName = $"PATCH {routePath}. With Invalid Email Returns 422 UnprocessableContent")]
	public async Task Patch_WithInvalidEmail_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);
		var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
		var request = new AccountPatchRequest
		{
			Email = "existinguser.test"
		};
		var response = await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.UnprocessableContent);
	}

	
	[Fact(DisplayName = $"PATCH {routePath}. With Invalid Password Returns 422 UnprocessableContent")]
	public async Task Patch_WithInvalidPassword_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);

		var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
		var request = new AccountPatchRequest
		{
			Password = "invalid"
		};

		var response = await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.UnprocessableContent);
	}
	
	[Fact(DisplayName = $"PATCH {routePath}. With Empty Request Returns 422 UnprocessableContent")]
	public async Task Patch_WithEmptyRequest_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);

		var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
		var request = new AccountPatchRequest
		{
		};

		var response = await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.UnprocessableContent);
	}
	
	[Fact(DisplayName = $"PATCH {routePath}. With Invalid Id Returns 422 UnprocessableContent")]
	public async Task Patch_WithInvalidId_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);

		var url = routePath.Replace("{id}", "invalid");

		var request = new AccountPatchRequest
		{
			Username = this.existingAccountCredentials.Username,
			Email = this.existingAccountCredentials.Email,
			Password = this.existingAccountCredentials.Password,
			Status =  AccountStatus.Active,
			Role = AccountRole.User
		};

		var response = await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.UnprocessableContent);
	}

	#endregion

	public override async Task InitializeAsync()
	{
		await this.EnsureAccountExists(this.existingAccountCredentials);
		await base.InitializeAsync();
	}

	public override async Task DisposeAsync()
	{
		await base.DisposeAsync();
	}

}
