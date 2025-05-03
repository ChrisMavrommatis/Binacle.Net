using System.Net;
using System.Net.Http.Json;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.IntegrationTests.ExtensionMethods;
using Binacle.Net.ServiceModule.IntegrationTests.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin.Subscription;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
[Collection(BinacleApiAsAServiceCollection.Name)]
public class Create : AdminEndpointsTestsBase
{
	private readonly AccountCredentials accountCredentialsWithSubscription;
	private readonly AccountCredentials accountCredentialsWithoutSubscription;

	public Create(BinacleApiAsAServiceFactory sut) : base(sut)
	{
		this.accountCredentialsWithSubscription = new AccountCredentials()
		{
			Username = "existinguser@binacle.net",
			Email = "existinguser@binacle.net",
			Password = "Ex1stingUs3rP@ssw0rd"
		};

		this.accountCredentialsWithoutSubscription = new AccountCredentials
		{
			Username = "newuser@binacle.net",
			Email = "newuser@binacle.net",
			Password = "N3wUs3rP@ssw0rd"
		};
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
			var url = routePath.Replace("{id}", this.accountCredentialsWithSubscription.Id.ToString());

			return await this.Sut.Client.PostAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});


	[Fact(DisplayName = $"POST {routePath}. With Expired Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithExpiredBearerToken_Returns_401Unauthorized()
		=> this.Action_WithExpiredBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new SubscriptionCreateRequest()
			{
				Type = SubscriptionType.Normal
			};
			var url = routePath.Replace("{id}", this.accountCredentialsWithSubscription.Id.ToString());

			return await this.Sut.Client.PostAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});

	[Fact(DisplayName = $"POST {routePath}. With Wrong Issuer Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithWrongIssuerBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new SubscriptionCreateRequest()
			{
				Type = SubscriptionType.Normal
			};
			var url = routePath.Replace("{id}", this.accountCredentialsWithSubscription.Id.ToString());

			return await this.Sut.Client.PostAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});


	[Fact(DisplayName = $"POST {routePath}. With Wrong Audience Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithWrongAudienceBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new SubscriptionCreateRequest()
			{
				Type = SubscriptionType.Normal
			};
			var url = routePath.Replace("{id}", this.accountCredentialsWithSubscription.Id.ToString());

			return await this.Sut.Client.PostAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});


	[Fact(DisplayName = $"POST {routePath}. With Wrongly Signed Bearer Token Returns 401 Unauthorized")]
	public Task Post_WithWronglySignedBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWronglySignedBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new SubscriptionCreateRequest()
			{
				Type = SubscriptionType.Normal
			};
			var url = routePath.Replace("{id}", this.accountCredentialsWithSubscription.Id.ToString());

			return await this.Sut.Client.PostAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
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
			var url = routePath.Replace("{id}", this.accountCredentialsWithSubscription.Id.ToString());

			return await this.Sut.Client.PostAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		});

	#endregion

	#region 201 Created

	[Fact(DisplayName = $"POST {routePath}. With Valid Credentials Returns 201 Created")]
	public async Task Post_WithValidCredentials_Returns_201Created()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);

		var request = new SubscriptionCreateRequest()
		{
			Type = SubscriptionType.Normal
		};
		var url = routePath.Replace("{id}", this.accountCredentialsWithoutSubscription.Id.ToString());

		var response = await this.Sut.Client.PostAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(HttpStatusCode.Created);
	}

	#endregion

	#region 404 Not Found

	[Fact(DisplayName = $"POST {routePath}. For Non Existing Account Returns 404 Not Found")]
	public async Task Post_ForNonExistingAccount_Returns_404NotFound()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);

		var nonExistentId = Guid.Parse("EF81C267-A003-44B8-AD89-4B48661C4AA5");

		var request = new SubscriptionCreateRequest()
		{
			Type = SubscriptionType.Normal
		};
		var url = routePath.Replace("{id}", nonExistentId.ToString());

		var response = await this.Sut.Client.PostAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
	}

	#endregion

	#region 409 Conflict

	[Fact(DisplayName = $"POST {routePath}. For Account With Subscription Returns 409 Conflict")]
	public async Task Post_ForAccountWithSubscription_Returns_409Conflict()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);

		var request = new SubscriptionCreateRequest()
		{
			Type = SubscriptionType.Normal
		};
		var url = routePath.Replace("{id}", this.accountCredentialsWithSubscription.Id.ToString());

		var response = await this.Sut.Client.PostAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
	}

	#endregion

	#region 422 Unprocessable Content

	[Fact(DisplayName = $"POST {routePath}. With Invalid Id Returns 422 Unprocessable Content")]
	public async Task Post_WithInvalidId_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);
		var request = new SubscriptionCreateRequest()
		{
			Type = SubscriptionType.Normal
		};
		var url = routePath.Replace("{id}", "invalid");

		var response = await this.Sut.Client.PostAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);
	}
	
	[Fact(DisplayName = $"POST {routePath}. With Invalid Request Type Returns 422 Unprocessable Content")]
	public async Task Post_WithInvalidRequestType_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.AdminAccount);
		var request = new 
		{
			Type = "invalid"
		};
		var url = routePath.Replace("{id}", this.accountCredentialsWithSubscription.Id.ToString());

		var response = await this.Sut.Client.PostAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
		response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);
	}

	#endregion
	
	public override async ValueTask InitializeAsync()
	{
		await this.EnsureAccountExists(this.accountCredentialsWithSubscription);
		await this.EnsureAccountHasSubscription(this.accountCredentialsWithSubscription);
		await this.EnsureAccountExists(this.accountCredentialsWithoutSubscription);
		await base.InitializeAsync();
	}

	public override async ValueTask DisposeAsync()
	{
		await this.EnsureAccountDoesNotHaveSubscription(this.accountCredentialsWithoutSubscription);
		await base.DisposeAsync();
	}
}
