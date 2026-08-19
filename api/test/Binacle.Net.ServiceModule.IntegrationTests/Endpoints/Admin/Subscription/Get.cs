using System.Net;
using System.Text.Json;
using Binacle.Net.ServiceModule.IntegrationTests.ExtensionMethods;
using Binacle.Net.ServiceModule.IntegrationTests.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin.Subscription;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
public class Get : AdminEndpointsTestsBase
{
	private readonly AccountCredentialsWithSubscription accountCredentialsUnderTest;

	public Get(BinacleApi sut) : base(sut)
	{
		this.accountCredentialsUnderTest = new AccountCredentialsWithSubscription(
			Guid.Parse("4E3D9F4B-AC75-4E6D-AF88-5A3B0D9C4F64"),
			"subscriptiongetuser@test.binacle.net",
			"subscriptiongetuser@test.binacle.net",
			"SubscriptionG3tUs3ersP@ssw0rd",
			Guid.Parse("5F4EAF5C-BD86-4F7E-B099-6B4C1EAD5A75")
		);
	}

	private const string routePath = "/api/admin/account/{id}/subscription";

	#region 401 Unauthorized

	[Fact(DisplayName = $"GET {routePath}. Without Bearer Token Returns 401 Unauthorized")]
	public Task Get_WithoutBearerToken_Returns_401Unauthorized()
		=> this.Action_WithoutBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());
			return await this.Client.GetAsync(url, TestContext.Current.CancellationToken);
		});

	[Fact(DisplayName = $"GET {routePath}. With Expired Bearer Token Returns 401 Unauthorized")]
	public Task Get_WithExpiredBearerToken_Returns_401Unauthorized()
		=> this.Action_WithExpiredBearerToken_Returns_401Unauthorized(async () =>
		{
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());
			return await this.Client.GetAsync(url, TestContext.Current.CancellationToken);
		});

	#endregion

	#region 403 Forbidden

	[Fact(DisplayName = $"GET {routePath}. Without Admin Bearer Token Returns 403 Forbidden")]
	public Task Get_WithoutAdminBearerToken_Returns_403Forbidden()
		=> this.Action_WithoutAdminBearerToken_Returns_403Forbidden(async () =>
		{
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());
			return await this.Client.GetAsync(url, TestContext.Current.CancellationToken);
		});

	#endregion

	#region 200 OK

	[Fact(DisplayName = $"GET {routePath}. For Account With Subscription Returns 200 OK")]
	public async Task Get_ForAccountWithSubscription_Returns_200OK()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Client, this.Sut.Admin);

		var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

		var response = await this.Client.GetAsync(url, TestContext.Current.CancellationToken);
		response.StatusCode.ShouldBe(HttpStatusCode.OK);

		var raw = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
		var subscription = JsonSerializer.Deserialize<SubscriptionGetResponse>(
			raw,
			this.Sut.JsonSerializerOptions
		);

		subscription.ShouldNotBeNull();
		subscription.Id.ShouldBe(this.accountCredentialsUnderTest.SubscriptionId);
		subscription.AccountId.ShouldBe(this.accountCredentialsUnderTest.Id);
	}

	#endregion

	#region 404 Not Found

	[Fact(DisplayName = $"GET {routePath}. For Account Without Subscription Returns 404 Not Found")]
	public async Task Get_ForAccountWithoutSubscription_Returns_404NotFound()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Client, this.Sut.Admin);

		var url = routePath.Replace("{id}", this.Sut.User.Id.ToString());

		var response = await this.Client.GetAsync(url, TestContext.Current.CancellationToken);
		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
	}

	[Fact(DisplayName = $"GET {routePath}. For Non Existing Account Returns 404 Not Found")]
	public async Task Get_ForNonExistingAccount_Returns_404NotFound()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Client, this.Sut.Admin);

		var url = routePath.Replace("{id}", this.Sut.NonExistentId.ToString());

		var response = await this.Client.GetAsync(url, TestContext.Current.CancellationToken);
		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
	}

	#endregion

	#region 422 Unprocessable Content

	[Fact(DisplayName = $"GET {routePath}. With Invalid Id Returns 422 UnprocessableContent")]
	public async Task Get_WithInvalidId_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Client, this.Sut.Admin);

		var url = routePath.Replace("{id}", "invalid");

		var response = await this.Client.GetAsync(url, TestContext.Current.CancellationToken);
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
