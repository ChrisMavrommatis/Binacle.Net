using System.Net;
using Binacle.Net.ServiceModule.IntegrationTests.ExtensionMethods;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin.Subscription;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
public class List : AdminEndpointsTestsBase
{
	public List(BinacleApi sut) : base(sut)
	{
		
	}
	private const string routePath = "/api/admin/subscription/";

	#region 401 Unauthorized

	[Fact(DisplayName = $"GET {routePath}. Without Bearer Token Returns 401 Unauthorized")]
	public Task Get_WithoutBearerToken_Returns_401Unauthorized()
		=> this.Action_WithoutBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath;
			return await this.Sut.Client.GetAsync(url, TestContext.Current.CancellationToken);
		});

	[Fact(DisplayName = $"GET {routePath}. With Expired Bearer Token Returns 401 Unauthorized")]
	public Task Get_WithExpiredBearerToken_Returns_401Unauthorized()
		=> this.Action_WithExpiredBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath;
			return await this.Sut.Client.GetAsync(url, TestContext.Current.CancellationToken);
		});


	[Fact(DisplayName = $"GET {routePath}. With Wrong Issuer Bearer Token Returns 401 Unauthorized")]
	public Task Get_WithWrongIssuerBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath;
			return await this.Sut.Client.GetAsync(url, TestContext.Current.CancellationToken);
		});

	[Fact(DisplayName = $"GET {routePath}. With Wrong Audience Bearer Token Returns 401 Unauthorized")]
	public Task Get_WithWrongAudienceBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongAudienceBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath;
			return await this.Sut.Client.GetAsync(url, TestContext.Current.CancellationToken);
		});

	[Fact(DisplayName = $"GET {routePath}. With Wrongly Signed Bearer Token Returns 401 Unauthorized")]
	public Task Get_WithWronglySignedBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWronglySignedBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath;
			return await this.Sut.Client.GetAsync(url, TestContext.Current.CancellationToken);
		});

	#endregion

	#region 403 Forbidden

	[Fact(DisplayName = $"GET {routePath}. Without Admin Bearer Token Returns 403 Forbidden")]
	public Task Get_WithoutAdminBearerToken_Returns_403Forbidden()
		=> this.Action_WithoutAdminBearerToken_Returns_403Forbidden(async () =>
		{
			var url = routePath;
			return await this.Sut.Client.GetAsync(url, TestContext.Current.CancellationToken);
		});

	#endregion

	#region 20O OK

	[Fact(DisplayName = $"GET {routePath}. With With Admin Returns 200 OK")]
	public async Task Get_WithAdmin_Returns_200OK()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);

		var url = routePath;
		var response = await this.Sut.Client.GetAsync(url, TestContext.Current.CancellationToken);
		response.StatusCode.ShouldBe(HttpStatusCode.OK);
	}
	
	#endregion

	
	#region 422 Unprocessable Content
	
	[Fact(DisplayName = $"GET {routePath}. With Invalid Page Returns 422 UnprocessableContent")]
	public async Task Get_WithInvalidPage_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);
		var url = routePath + "?pg=0";
		var response = await this.Sut.Client.GetAsync(url, TestContext.Current.CancellationToken);
		response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);
		
		var url2 = routePath + "?pg=-1";
		var response2 = await this.Sut.Client.GetAsync(url2, TestContext.Current.CancellationToken);
		response2.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);
	}
	
	[Fact(DisplayName = $"GET {routePath}. With Invalid Page Size Returns 422 UnprocessableContent")]
	public async Task Get_WithInvalidPageSize_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);
		var url = routePath + "?pz=0";
		var response = await this.Sut.Client.GetAsync(url, TestContext.Current.CancellationToken);
		response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);

		var url2 = routePath + "?pz=-1";
		var response2 = await this.Sut.Client.GetAsync(url2, TestContext.Current.CancellationToken);
		response2.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);
	}

	#endregion
}
