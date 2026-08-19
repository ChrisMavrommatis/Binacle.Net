using System.Net;
using System.Text.Json;
using Binacle.Net.ServiceModule.IntegrationTests.ExtensionMethods;
using Binacle.Net.ServiceModule.IntegrationTests.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin.Subscription;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
public class List : AdminEndpointsTestsBase
{
	private readonly AccountCredentialsWithSubscription accountCredentialsUnderTest;

	public List(BinacleApi sut) : base(sut)
	{
		this.accountCredentialsUnderTest = new AccountCredentialsWithSubscription(
			Guid.Parse("2C1B7D2F-8A53-4C4B-8D66-3E1F8B7A2D42"),
			"subscriptionlistuser@test.binacle.net",
			"subscriptionlistuser@test.binacle.net",
			"SubscriptionL1stUs3ersP@ssw0rd",
			Guid.Parse("3D2C8E3A-9B64-4D5C-9E77-4F2A9C8B3E53")
		);
	}

	private const string routePath = "/api/admin/subscriptions";

	#region 401 Unauthorized

	[Fact(DisplayName = $"GET {routePath}. Without Bearer Token Returns 401 Unauthorized")]
	public Task List_WithoutBearerToken_Returns_401Unauthorized()
		=> this.Action_WithoutBearerToken_Returns_401Unauthorized(
			async () => await this.Client.GetAsync(routePath, TestContext.Current.CancellationToken)
		);

	[Fact(DisplayName = $"GET {routePath}. With Expired Bearer Token Returns 401 Unauthorized")]
	public Task List_WithExpiredBearerToken_Returns_401Unauthorized()
		=> this.Action_WithExpiredBearerToken_Returns_401Unauthorized(
			async () => await this.Client.GetAsync(routePath, TestContext.Current.CancellationToken)
		);

	#endregion

	#region 403 Forbidden

	[Fact(DisplayName = $"GET {routePath}. Without Admin Bearer Token Returns 403 Forbidden")]
	public Task List_WithoutAdminBearerToken_Returns_403Forbidden()
		=> this.Action_WithoutAdminBearerToken_Returns_403Forbidden(
			async () => await this.Client.GetAsync(routePath, TestContext.Current.CancellationToken)
		);

	#endregion

	#region 200 OK

	[Fact(DisplayName = $"GET {routePath}. With Admin Bearer Token Returns 200 OK")]
	public async Task List_WithAdminBearerToken_Returns_200OK()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Client, this.Sut.Admin);

		var response = await this.Client.GetAsync(
			$"{routePath}?pageSize=200",
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.OK);

		var raw = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
		var page = JsonSerializer.Deserialize<PagedResponse<SubscriptionGetResponse>>(
			raw,
			this.Sut.JsonSerializerOptions
		);

		page.ShouldNotBeNull();
		page.Page.ShouldBe(1);
		page.Total.ShouldBeGreaterThan(0);

		var subscription = page.Items.SingleOrDefault(
			x => x.Id == this.accountCredentialsUnderTest.SubscriptionId
		);
		subscription.ShouldNotBeNull();
		subscription.AccountId.ShouldBe(this.accountCredentialsUnderTest.Id);
	}

	#endregion

	#region 422 Unprocessable Content

	[Theory(DisplayName = $"GET {routePath}. With Out Of Range Page Size Returns 422 UnprocessableContent")]
	[InlineData(0)]
	[InlineData(201)]
	public async Task List_WithOutOfRangePageSize_Returns_422UnprocessableContent(int pageSize)
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Client, this.Sut.Admin);

		var response = await this.Client.GetAsync(
			$"{routePath}?pageSize={pageSize}",
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
