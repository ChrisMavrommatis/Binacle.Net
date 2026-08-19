using System.Net;
using System.Text.Json;
using Binacle.Net.ServiceModule.IntegrationTests.ExtensionMethods;
using Binacle.Net.ServiceModule.IntegrationTests.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin.Account;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
public class List : AdminEndpointsTestsBase
{
	private readonly AccountCredentials accountCredentialsUnderTest;

	public List(BinacleApi sut) : base(sut)
	{
		this.accountCredentialsUnderTest = new AccountCredentials(
			Guid.Parse("1B0A6C1E-7F42-4B3A-9C55-2D0E7A6F1C31"),
			"listuser@test.binacle.net",
			"listuser@test.binacle.net",
			"L1stUs3ersP@ssw0rd"
		);
	}

	private const string routePath = "/api/admin/accounts";

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

	[Fact(DisplayName = $"GET {routePath}. With Wrongly Signed Bearer Token Returns 401 Unauthorized")]
	public Task List_WithWronglySignedBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWronglySignedBearerToken_Returns_401Unauthorized(
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

		// The list must never carry credentials. AccountListItem has no such field - this catches it coming back.
		raw.Contains("passwordHash", StringComparison.OrdinalIgnoreCase).ShouldBeFalse();
		raw.Contains("securityStamp", StringComparison.OrdinalIgnoreCase).ShouldBeFalse();

		var page = JsonSerializer.Deserialize<PagedResponse<AccountListItem>>(raw, this.Sut.JsonSerializerOptions);

		page.ShouldNotBeNull();
		page.Page.ShouldBe(1);
		page.PageSize.ShouldBe(200);
		page.Total.ShouldBeGreaterThan(0);
		page.Items.ShouldContain(x => x.Id == this.accountCredentialsUnderTest.Id);
	}

	[Fact(DisplayName = $"GET {routePath}. Pages Through Accounts One At A Time")]
	public async Task List_WithPageSizeOne_Pages_Through_Accounts()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Client, this.Sut.Admin);

		var firstPage = await this.GetPageAsync(page: 1, pageSize: 1);
		var secondPage = await this.GetPageAsync(page: 2, pageSize: 1);

		firstPage.PageSize.ShouldBe(1);
		firstPage.Items.Count.ShouldBe(1);
		firstPage.TotalPages.ShouldBe(firstPage.Total);

		// The suite always seeds an admin and at least one user, so a second page exists.
		secondPage.Items.Count.ShouldBe(1);
		secondPage.Items[0].Id.ShouldNotBe(firstPage.Items[0].Id);
	}

	private async Task<PagedResponse<AccountListItem>> GetPageAsync(int page, int pageSize)
	{
		var response = await this.Client.GetAsync(
			$"{routePath}?page={page}&pageSize={pageSize}",
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.OK);

		var result = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
		var deserialized = JsonSerializer.Deserialize<PagedResponse<AccountListItem>>(
			result,
			this.Sut.JsonSerializerOptions
		);
		deserialized.ShouldNotBeNull();
		return deserialized;
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

	[Fact(DisplayName = $"GET {routePath}. With Zero Page Returns 422 UnprocessableContent")]
	public async Task List_WithZeroPage_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Client, this.Sut.Admin);

		var response = await this.Client.GetAsync(
			$"{routePath}?page=0",
			TestContext.Current.CancellationToken
		);
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
