using Binacle.Net.ServiceModule.IntegrationTests.Models;
using System.Net.Http.Json;
using Binacle.Net.ServiceModule.Domain.Accounts.Models;
using Binacle.Net.ServiceModule.IntegrationTests.ExtensionMethods;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin.Account;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
[Collection(BinacleApiAsAServiceCollection.Name)]
public class List : AdminEndpointsTestsBase
{
	public List(BinacleApiAsAServiceFactory sut) : base(sut)
	{
		
	}
	private const string routePath = "/api/admin/account/";

	#region 401 Unauthorized

	[Fact(DisplayName = $"GET {routePath}. Without Bearer Token Returns 401 Unauthorized")]
	public Task Get_WithoutBearerToken_Returns_401Unauthorized()
		=> this.Action_WithoutBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath;
			return await this.Sut.Client.GetAsync(url);
		});

	[Fact(DisplayName = $"GET {routePath}. With Expired Bearer Token Returns 401 Unauthorized")]
	public Task Get_WithExpiredBearerToken_Returns_401Unauthorized()
		=> this.Action_WithExpiredBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath;
			return await this.Sut.Client.GetAsync(url);
		});


	[Fact(DisplayName = $"GET {routePath}. With Wrong Issuer Bearer Token Returns 401 Unauthorized")]
	public Task Get_WithWrongIssuerBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath;
			return await this.Sut.Client.GetAsync(url);
		});

	[Fact(DisplayName = $"GET {routePath}. With Wrong Audience Bearer Token Returns 401 Unauthorized")]
	public Task Get_WithWrongAudienceBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongAudienceBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath;
			return await this.Sut.Client.GetAsync(url);
		});

	[Fact(DisplayName = $"GET {routePath}. With Wrongly Signed Bearer Token Returns 401 Unauthorized")]
	public Task Get_WithWronglySignedBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWronglySignedBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath;
			return await this.Sut.Client.GetAsync(url);
		});

	#endregion

	#region 403 Forbidden

	[Fact(DisplayName = $"GET {routePath}. Without Admin Bearer Token Returns 403 Forbidden")]
	public Task Get_WithoutAdminBearerToken_Returns_403Forbidden()
		=> this.Action_WithoutAdminBearerToken_Returns_403Forbidden(async () =>
		{
			var url = routePath;
			return await this.Sut.Client.GetAsync(url);
		});

	#endregion

	#region 20O OK

	[Fact(DisplayName = $"GET {routePath}. With Existing Account Returns 200 OK")]
	public async Task Get_WithExistingAccount_Returns_200OK()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);

		var url = routePath;
		var response = await this.Sut.Client.GetAsync(url);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
	}
	
	#endregion

	#region 404 Not Found

	[Fact(DisplayName = $"GET {routePath}. For Non Existing User Returns 404 Not Found")]
	public async Task  Get_WhenRequestingPageSizeThatDoesntExist_Returns_404NotFound()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);
		var url = routePath.Concat("?pg=10").ToString();
		
		var response = await this.Sut.Client.GetAsync(url);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
	}

	#endregion
	
	#region 422 Unprocessable Content
	
	[Fact(DisplayName = $"GET {routePath}. With Invalid Page Returns 422 UnprocessableContent")]
	public async Task Get_WithInvalidPage_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);
		var url = routePath + "?pg=0";
		var response = await this.Sut.Client.GetAsync(url);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.UnprocessableContent);
		
		var url2 = routePath + "?pg=-1";
		var response2 = await this.Sut.Client.GetAsync(url2);
		response2.StatusCode.ShouldBe(System.Net.HttpStatusCode.UnprocessableContent);
	}
	
	[Fact(DisplayName = $"GET {routePath}. With Invalid Page Size Returns 422 UnprocessableContent")]
	public async Task Get_WithInvalidPageSize_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);
		var url = routePath + "?pz=0";
		var response = await this.Sut.Client.GetAsync(url);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.UnprocessableContent);

		var url2 = routePath + "?pz=-1";
		var response2 = await this.Sut.Client.GetAsync(url2);
		response2.StatusCode.ShouldBe(System.Net.HttpStatusCode.UnprocessableContent);
	}

	#endregion
}
