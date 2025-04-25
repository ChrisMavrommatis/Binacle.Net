using System.Net;
using Binacle.Net.ServiceModule.IntegrationTests.ExtensionMethods;
using Binacle.Net.ServiceModule.IntegrationTests.Models;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin.Account;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
[Collection(BinacleApiAsAServiceCollection.Name)]
public class Delete : AdminEndpointsTestsBase
{
	private readonly AccountCredentials existingAccountCredentials;
	private readonly AccountCredentials newAccountCredentials;

	public Delete(BinacleApiAsAServiceFactory sut) : base(sut)
	{
		this.newAccountCredentials = new AccountCredentials
		{
			Username = "newuser@binacle.net",
			Email =  "newuser@binacle.net",
			Password = "N3wUs3rP@ssw0rd"
		};
		
		this.existingAccountCredentials = new AccountCredentials()
		{
			Username =  "existinguser@binacle.net",
			Email = "existinguser@binacle.net",
			Password = "Ex1stingUs3rP@ssw0rd"

		};
	}
	private const string routePath = "/api/admin/account/{id}";

	#region 401 Unauthorized

	[Fact(DisplayName = $"DELETE {routePath}. Without Bearer Token Returns 401 Unauthorized")]
	public Task Delete_WithoutBearerToken_Returns_401Unauthorized()
		=> this.Action_WithoutBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			return await this.Sut.Client.DeleteAsync(url);
		});

	[Fact(DisplayName = $"DELETE {routePath}. With Expired Bearer Token Returns 401 Unauthorized")]
	public Task Delete_WithExpiredBearerToken_Returns_401Unauthorized()
		=> this.Action_WithExpiredBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			return await this.Sut.Client.DeleteAsync(url);
		});


	[Fact(DisplayName = $"DELETE {routePath}. With Wrong Issuer Bearer Token Returns 401 Unauthorized")]
	public Task Delete_WithWrongIssuerBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			return await this.Sut.Client.DeleteAsync(url);
		});

	[Fact(DisplayName = $"DELETE {routePath}. With Wrong Audience Bearer Token Returns 401 Unauthorized")]
	public Task Delete_WithWrongAudienceBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongAudienceBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			return await this.Sut.Client.DeleteAsync(url);
		});

	[Fact(DisplayName = $"DELETE {routePath}. With Wrongly Signed Bearer Token Returns 401 Unauthorized")]
	public Task Delete_WithWronglySignedBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWronglySignedBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			return await this.Sut.Client.DeleteAsync(url);
		});

	#endregion

	#region 403 Forbidden

	[Fact(DisplayName = $"DELETE {routePath}. Without Admin Bearer Token Returns 403 Forbidden")]
	public Task Delete_WithoutAdminBearerToken_Returns_403Forbidden()
		=> this.Action_WithoutAdminBearerToken_Returns_403Forbidden(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			return await this.Sut.Client.DeleteAsync(url);
		});
	
	#endregion

	#region 204 No Content

	[Fact(DisplayName = $"DELETE {routePath}. With Valid Credentials Returns 204 No Content")]
	public async Task Delete_WithValidCredentials_Returns_204NoContent()
	{
		await this.EnsureAccountExists(this.newAccountCredentials);

		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);
		
		var url = routePath.Replace("{id}", this.newAccountCredentials.Id.ToString());
		var response = await this.Sut.Client.DeleteAsync(url);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
	}
	#endregion

	#region 400 Bad Request

	[Fact(DisplayName = $"DELETE {routePath}. With Invalid Id Returns 400 BadRequest")]
	public async Task Delete_WithInvalidId_Returns_400BadRequest()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);

		var url = routePath.Replace("{id}", "invalid");
		var response = await this.Sut.Client.DeleteAsync(url);
		response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
	}
	
	#endregion

	#region 404 Not Found

	[Fact(DisplayName = $"DELETE {routePath}. For Non Existing User Returns 404 Not Found")]
	public async Task Delete_ForNonExistingUser_Returns_404NotFound()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);
		var nonExistentId = Guid.Parse("EF81C267-A003-44B8-AD89-4B48661C4AA5");

		var url = routePath.Replace("{id}", nonExistentId.ToString());
		var response = await this.Sut.Client.DeleteAsync(url);
		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
	}

	#endregion


	public override async Task InitializeAsync()
	{
		await this.EnsureAccountExists(this.existingAccountCredentials);
		await base.InitializeAsync();
	}

	public override async Task DisposeAsync()
	{
		await this.EnsureAccountDoesNotExist(this.existingAccountCredentials);
		await base.DisposeAsync();
	}
}
