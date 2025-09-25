using System.Net;
using System.Net.Http.Json;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.IntegrationTests.ExtensionMethods;
using Binacle.Net.ServiceModule.IntegrationTests.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin.Subscription;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
public class Create : AdminEndpointsTestsBase
{
	private readonly AccountCredentials accountCredentialsUnderTest;
	private readonly AccountCredentialsWithSubscription accountCredentialsWithSubscriptionUnderTest;

	public Create(BinacleApi sut) : base(sut)
	{
		this.accountCredentialsUnderTest = new AccountCredentials(
			Guid.Parse("3AD36BF1-9791-4899-AF6F-EB1F666D42D0"),
			"subscriptioncreateuser@test.binacle.net",
			"subscriptioncreateuser@test.binacle.net",
			"SubscriptionCr34teUs3ersP@ssw0rd"
		);
		
		this.accountCredentialsWithSubscriptionUnderTest = new AccountCredentialsWithSubscription(
			Guid.Parse("283DA5A3-6670-4AAE-887C-59B5387CE897"),
			"subscriptioneuser@test.binacle.net",
			"subscriptioneuser@test.binacle.net",
			"SubscriptionUs3ersP@ssw0rd",
			Guid.Parse("D938CB5B-1288-4B40-8BE0-CB84597ED8B3")
		);
	}

	private const string routePath = "/api/admin/account/{id}/subscription";

	#region 401 Unauthorized

	[Fact(DisplayName = $"POST {routePath}. Without Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithoutBearerToken_Returns_401Unauthorized()
		=> this.Action_WithoutBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new SubscriptionCreateRequest()
			{
				Type = SubscriptionType.Normal
			};
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Sut.Client.PostAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions, 
				TestContext.Current.CancellationToken
			);
		});


	[Fact(DisplayName = $"POST {routePath}. With Expired Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithExpiredBearerToken_Returns_401Unauthorized()
		=> this.Action_WithExpiredBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new SubscriptionCreateRequest()
			{
				Type = SubscriptionType.Normal
			};
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Sut.Client.PostAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions, 
				TestContext.Current.CancellationToken
			);
		});

	[Fact(DisplayName = $"POST {routePath}. With Wrong Issuer Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithWrongIssuerBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new SubscriptionCreateRequest()
			{
				Type = SubscriptionType.Normal
			};
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Sut.Client.PostAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions, 
				TestContext.Current.CancellationToken
			);
		});


	[Fact(DisplayName = $"POST {routePath}. With Wrong Audience Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithWrongAudienceBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new SubscriptionCreateRequest()
			{
				Type = SubscriptionType.Normal
			};
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Sut.Client.PostAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions, 
				TestContext.Current.CancellationToken
			);
		});


	[Fact(DisplayName = $"POST {routePath}. With Wrongly Signed Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithWronglySignedBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWronglySignedBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new SubscriptionCreateRequest()
			{
				Type = SubscriptionType.Normal
			};
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Sut.Client.PostAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions, 
				TestContext.Current.CancellationToken
			);
		});

	#endregion

	#region 403 Forbidden

	[Fact(DisplayName = $"POST {routePath}. Without Admin Bearer Token Returns 403 Forbidden")]
	public Task Post_WithoutAdminBearerToken_Returns_403Forbidden()
		=> this.Action_WithoutAdminBearerToken_Returns_403Forbidden(async () =>
		{
			var request = new SubscriptionCreateRequest()
			{
				Type = SubscriptionType.Normal
			};
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Sut.Client.PostAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions, 
				TestContext.Current.CancellationToken
			);
		});

	#endregion

	#region 201 Created

	[Fact(DisplayName = $"POST {routePath}. With Valid Request Returns 201 Created")]
	public async Task Post_WithValidRequest_Returns_201Created()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);

		var request = new SubscriptionCreateRequest()
		{
			Type = SubscriptionType.Normal
		};
		var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

		var response = await this.Sut.Client.PostAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions, 
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.Created);
	}

	#endregion

	#region 404 Not Found

	[Fact(DisplayName = $"POST {routePath}. For Non Existing Account Returns 404 Not Found")]
	public async Task Post_ForNonExistingAccount_Returns_404NotFound()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);

		var request = new SubscriptionCreateRequest()
		{
			Type = SubscriptionType.Normal
		};
		var url = routePath.Replace("{id}", this.Sut.NonExistentId.ToString());

		var response = await this.Sut.Client.PostAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions, 
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
	}

	#endregion

	#region 409 Conflict

	[Fact(DisplayName = $"POST {routePath}. For Account With Subscription Returns 409 Conflict")]
	public async Task Post_ForAccountWithSubscription_Returns_409Conflict()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);

		var request = new SubscriptionCreateRequest()
		{
			Type = SubscriptionType.Normal
		};
		var url = routePath.Replace("{id}", this.accountCredentialsWithSubscriptionUnderTest.Id.ToString());

		var response = await this.Sut.Client.PostAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions, 
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
	}

	#endregion

	#region 422 Unprocessable Content

	[Fact(DisplayName = $"POST {routePath}. With Invalid Id Returns 422 Unprocessable Content")]
	public async Task Post_WithInvalidId_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);
		var request = new SubscriptionCreateRequest()
		{
			Type = SubscriptionType.Normal
		};
		var url = routePath.Replace("{id}", "invalid");

		var response = await this.Sut.Client.PostAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions, 
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);
	}
	
	[Fact(DisplayName = $"POST {routePath}. With Invalid Subscription Type Returns 422 Unprocessable Content")]
	public async Task Post_WithInvalidSubscriptionType_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);
		var request = new 
		{
			Type = "invalid"
		};
		var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

		var response = await this.Sut.Client.PostAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions, 
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);
	}

	#endregion
	
	public override async ValueTask InitializeAsync()
	{
		await this.Sut.EnsureAccountExists(this.accountCredentialsUnderTest);
		await this.Sut.EnsureAccountExistsWithSubscription(this.accountCredentialsWithSubscriptionUnderTest);
		await base.InitializeAsync();
	}

	public override async ValueTask DisposeAsync()
	{
		await this.Sut.EnsureAccountDoesNotExist(this.accountCredentialsUnderTest);
		await this.Sut.EnsureAccountWithSubscriptionDoesNotExist(this.accountCredentialsWithSubscriptionUnderTest);
		await base.DisposeAsync();
	}
}
