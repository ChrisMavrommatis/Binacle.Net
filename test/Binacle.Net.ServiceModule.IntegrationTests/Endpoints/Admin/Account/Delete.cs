using System.Net;
using Binacle.Net.ServiceModule.IntegrationTests.ExtensionMethods;
using Binacle.Net.ServiceModule.IntegrationTests.Models;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin.Account;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
public class Delete : AdminEndpointsTestsBase
{
	private readonly AccountCredentials accountCredentialsUnderTest;

	public Delete(BinacleApi sut) : base(sut)
	{
		this.accountCredentialsUnderTest = new AccountCredentials(
			Guid.Parse("DEF02C0F-FB96-4799-8A4A-8427D5AF29DC"),
			"deleteuser@binacle.net",
			"deleteuser@binacle.net",
			"D3l3teUs3ersP@ssw0rd"
		);
	}
	private const string routePath = "/api/admin/account/{id}";

	#region 401 Unauthorized

	[Fact(DisplayName = $"DELETE {routePath}. Without Bearer Token Returns 401 Unauthorized")]
	public Task Delete_WithoutBearerToken_Returns_401Unauthorized()
		=> this.Action_WithoutBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());
			return await this.Sut.Client.DeleteAsync(url, TestContext.Current.CancellationToken);
		});

	[Fact(DisplayName = $"DELETE {routePath}. With Expired Bearer Token Returns 401 Unauthorized")]
	public Task Delete_WithExpiredBearerToken_Returns_401Unauthorized()
		=> this.Action_WithExpiredBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());
			return await this.Sut.Client.DeleteAsync(url, TestContext.Current.CancellationToken);
		});


	[Fact(DisplayName = $"DELETE {routePath}. With Wrong Issuer Bearer Token Returns 401 Unauthorized")]
	public Task Delete_WithWrongIssuerBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());
			return await this.Sut.Client.DeleteAsync(url, TestContext.Current.CancellationToken);
		});

	[Fact(DisplayName = $"DELETE {routePath}. With Wrong Audience Bearer Token Returns 401 Unauthorized")]
	public Task Delete_WithWrongAudienceBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongAudienceBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());
			return await this.Sut.Client.DeleteAsync(url, TestContext.Current.CancellationToken);
		});

	[Fact(DisplayName = $"DELETE {routePath}. With Wrongly Signed Bearer Token Returns 401 Unauthorized")]
	public Task Delete_WithWronglySignedBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWronglySignedBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());
			return await this.Sut.Client.DeleteAsync(url, TestContext.Current.CancellationToken);
		});

	#endregion

	#region 403 Forbidden

	[Fact(DisplayName = $"DELETE {routePath}. Without Admin Bearer Token Returns 403 Forbidden")]
	public Task Delete_WithoutAdminBearerToken_Returns_403Forbidden()
		=> this.Action_WithoutAdminBearerToken_Returns_403Forbidden(async () =>
		{
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());
			return await this.Sut.Client.DeleteAsync(url, TestContext.Current.CancellationToken);
		});
	
	#endregion

	#region 204 No Content

	[Fact(DisplayName = $"DELETE {routePath}. With Valid Credentials Returns 204 No Content")]
	public async Task Delete_WithValidCredentials_Returns_204NoContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);
		
		var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());
		var response = await this.Sut.Client.DeleteAsync(url, TestContext.Current.CancellationToken);
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
	}
	#endregion

	#region 404 Not Found

	[Fact(DisplayName = $"DELETE {routePath}. For Non Existing Account Returns 404 Not Found")]
	public async Task Delete_ForNonExistingAccount_Returns_404NotFound()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);

		var url = routePath.Replace("{id}", this.Sut.NonExistentId.ToString());
		var response = await this.Sut.Client.DeleteAsync(url, TestContext.Current.CancellationToken);
		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
	}

	#endregion
	
	#region 422 Unprocessable Content

	[Fact(DisplayName = $"DELETE {routePath}. With Invalid Id Returns 422 UnprocessableContent")]
	public async Task Delete_WithInvalidId_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);

		var url = routePath.Replace("{id}", "invalid");
		var response = await this.Sut.Client.DeleteAsync(url, TestContext.Current.CancellationToken);
		response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);
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
