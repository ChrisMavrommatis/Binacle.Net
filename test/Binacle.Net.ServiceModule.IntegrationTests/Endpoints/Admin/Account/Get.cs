using Binacle.Net.ServiceModule.IntegrationTests.Models;
using System.Net.Http.Json;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.IntegrationTests.ExtensionMethods;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin.Account;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
[Collection(BinacleApiAsAServiceCollection.Name)]
public class Get : AdminEndpointsTestsBase
{
	private readonly AccountCredentials existingAccountCredentials;

	public Get(BinacleApiAsAServiceFactory sut) : base(sut)
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

	[Fact(DisplayName = $"GET {routePath}. Without Bearer Token Returns 401 Unauthorized")]
	public Task Get_WithoutBearerToken_Returns_401Unauthorized()
		=> this.Action_WithoutBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			return await this.Sut.Client.GetAsync(url);
		});

	[Fact(DisplayName = $"GET {routePath}. With Expired Bearer Token Returns 401 Unauthorized")]
	public Task Get_WithExpiredBearerToken_Returns_401Unauthorized()
		=> this.Action_WithExpiredBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			return await this.Sut.Client.GetAsync(url);
		});


	[Fact(DisplayName = $"GET {routePath}. With Wrong Issuer Bearer Token Returns 401 Unauthorized")]
	public Task Get_WithWrongIssuerBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			return await this.Sut.Client.GetAsync(url);
		});

	[Fact(DisplayName = $"GET {routePath}. With Wrong Audience Bearer Token Returns 401 Unauthorized")]
	public Task Get_WithWrongAudienceBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongAudienceBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			return await this.Sut.Client.GetAsync(url);
		});

	[Fact(DisplayName = $"GET {routePath}. With Wrongly Signed Bearer Token Returns 401 Unauthorized")]
	public Task Get_WithWronglySignedBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWronglySignedBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			return await this.Sut.Client.GetAsync(url);
		});

	#endregion

	#region 403 Forbidden

	[Fact(DisplayName = $"GET {routePath}. Without Admin Bearer Token Returns 403 Forbidden")]
	public Task Get_WithoutAdminBearerToken_Returns_403Forbidden()
		=> this.Action_WithoutAdminBearerToken_Returns_403Forbidden(async () =>
		{
			var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
			return await this.Sut.Client.GetAsync(url);
		});

	#endregion

	#region 20O OK

	[Fact(DisplayName = $"GET {routePath}. With Existing Account Returns 200 OK")]
	public async Task Get_WithExistingAccount_Returns_200OK()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);

		var url = routePath.Replace("{id}", this.existingAccountCredentials.Id.ToString());
	
		var response = await this.Sut.Client.GetAsync(url);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
	}
	
	#endregion

	#region 404 Not Found

	[Fact(DisplayName = $"GET {routePath}. For Non Existing Account Returns 404 Not Found")]
	public async Task  Get_ForNonExistingAccount_Returns_404NotFound()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);
		var nonExistentId = Guid.Parse("EF81C267-A003-44B8-AD89-4B48661C4AA5");

		var url = routePath.Replace("{id}", nonExistentId.ToString());
		
		var response = await this.Sut.Client.GetAsync(url);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
	}

	#endregion
	
	#region 422 Unprocessable Content
	
	[Fact(DisplayName = $"GET {routePath}. With Invalid Id Returns 422 UnprocessableContent")]
	public async Task Get_WithInvalidId_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);
		var url = routePath.Replace("{id}", "invalid");

		var response = await this.Sut.Client.GetAsync(url);
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
