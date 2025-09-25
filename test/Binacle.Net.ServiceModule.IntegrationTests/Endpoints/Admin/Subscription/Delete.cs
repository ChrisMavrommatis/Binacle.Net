using System.Net;
using Binacle.Net.ServiceModule.IntegrationTests.ExtensionMethods;
using Binacle.Net.ServiceModule.IntegrationTests.Models;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin.Subscription;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
public class Delete : AdminEndpointsTestsBase
{
	private readonly AccountCredentialsWithSubscription accountCredentialsUnderTest;

	public Delete(BinacleApi sut) : base(sut)
	{
		this.accountCredentialsUnderTest = new AccountCredentialsWithSubscription(
			Guid.Parse("09E747ED-5F31-46D0-BC40-3A9DE774D667"),
			"subscriptiondeleteuser@test.binacle.net",
			"subscriptiondeleteuser@test.binacle.net",
			"SubscriptionD3l3teUs3ersP@ssw0rd",
			Guid.Parse("E639ED76-8382-46AD-A57B-069EAB59B412")
		);
	}

	private const string routePath = "/api/admin/account/{id}/subscription";

	#region 401 Unauthorized

	[Fact(DisplayName = $"DELETE {routePath}. Without Bearer Token Returns 401 Unauthorized")]
	public Task Delete_WithoutBearerToken_Returns_401Unauthorized()
		=> this.Action_WithoutBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Sut.Client.DeleteAsync(
				url,
				TestContext.Current.CancellationToken
			);
		});


	[Fact(DisplayName = $"DELETE {routePath}. With Expired Bearer Token Returns 401 Unauthorized")]
	public Task Delete_WithExpiredBearerToken_Returns_401Unauthorized()
		=> this.Action_WithExpiredBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Sut.Client.DeleteAsync(
				url,
				TestContext.Current.CancellationToken
			);
		});

	[Fact(DisplayName = $"DELETE {routePath}. With Wrong Issuer Bearer Token Returns 401 Unauthorized")]
	public Task Delete_WithWrongIssuerBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Sut.Client.DeleteAsync(
				url,
				TestContext.Current.CancellationToken
			);
		});


	[Fact(DisplayName = $"DELETE {routePath}. With Wrong Audience Bearer Token Returns 401 Unauthorized")]
	public Task Delete_WithWrongAudienceBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Sut.Client.DeleteAsync(
				url,
				TestContext.Current.CancellationToken
			);
		});


	[Fact(DisplayName = $"DELETE {routePath}. With Wrongly Signed Bearer Token Returns 401 Unauthorized")]
	public Task Delete_WithWronglySignedBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWronglySignedBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Sut.Client.DeleteAsync(
				url,
				TestContext.Current.CancellationToken
			);
		});

	#endregion

	#region 403 Forbidden

	[Fact(DisplayName = $"DELETE {routePath}. Without Admin Bearer Token Returns 403 Forbidden")]
	public Task Delete_WithoutAdminBearerToken_Returns_403Forbidden()
		=> this.Action_WithoutAdminBearerToken_Returns_403Forbidden(async () =>
		{
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Sut.Client.DeleteAsync(
				url,
				TestContext.Current.CancellationToken
			);
		});

	#endregion

	#region 204 No Content

	[Fact(DisplayName = $"DELETE {routePath}. For Account With Subscription Returns 204 No Content")]
	public async Task Delete_ForAccountWithSubscription_Returns_204NoContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);

		var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

		var response = await this.Sut.Client.DeleteAsync(
			url,
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
	}

	#endregion

	#region 404 Not Found

	[Fact(DisplayName = $"DELETE {routePath}. For Non Existing Account Returns 404 Not Found")]
	public async Task Delete_ForNonExistingAccount_Returns_404NotFound()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);

		var url = routePath.Replace("{id}", this.Sut.NonExistentId.ToString());

		var response = await this.Sut.Client.DeleteAsync(
			url,
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
	}
	
	[Fact(DisplayName = $"DELETE {routePath}. For Account Without Subscription Returns 404 Not Found")]
	public async Task Delete_ForAccountWithoutSubscription_Returns_404NotFound()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);

		var url = routePath.Replace("{id}", this.Sut.ExistingAccountCredentials.Id.ToString());

		var response = await this.Sut.Client.DeleteAsync(
			url,
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
	}

	#endregion

	#region 422 Unprocessable Content

	[Fact(DisplayName = $"DELETE {routePath}. With Invalid Id Returns 422 Unprocessable Content")]
	public async Task Delete_WithInvalidId_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);
		
		var url = routePath.Replace("{id}", "invalid");

		var response = await this.Sut.Client.DeleteAsync(
			url,
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);
	}
	
	#endregion
	
	public override async ValueTask InitializeAsync()
	{
		await this.Sut.EnsureAccountExistsWithSubscription(this.accountCredentialsUnderTest);
		await base.InitializeAsync();
	}

	public override async ValueTask DisposeAsync()
	{
		await this.Sut.EnsureAccountWithSubscriptionDoesNotExist(this.accountCredentialsUnderTest);
		await base.DisposeAsync();
	}
}
