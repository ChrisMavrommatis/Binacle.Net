using System.Net;
using System.Net.Http.Json;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.IntegrationTests.ExtensionMethods;
using Binacle.Net.ServiceModule.IntegrationTests.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin.Subscription;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
public class Update : AdminEndpointsTestsBase
{
	private readonly AccountCredentialsWithSubscription accountCredentialsUnderTest;

	public Update(BinacleApi sut) : base(sut)
	{
		this.accountCredentialsUnderTest = new AccountCredentialsWithSubscription(
			Guid.Parse("ECED0A9E-C1F4-4942-8352-D511414B757F"),
			"subscriptionupdateuser@binacle.net",
			"subscriptionupdateuser@binacle.net",
			"SubscriptionUpd4t3Us3ersP@ssw0rd",
			Guid.Parse("E9EA544C-6CC3-44FB-8D35-90374DFBF075")
		);
	}

	private const string routePath = "/api/admin/account/{id}/subscription";

	#region 401 Unauthorized

	[Fact(DisplayName = $"PUT {routePath}. Without Bearer Token Returns 401 Unauthorized")]
	public Task Put_WithoutBearerToken_Returns_401Unauthorized()
		=> this.Action_WithoutBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new SubscriptionUpdateRequest()
			{
				Type = SubscriptionType.Normal,
				Status = SubscriptionStatus.Active
			};
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Sut.Client.PutAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions, 
				TestContext.Current.CancellationToken
			);
		});


	[Fact(DisplayName = $"PUT {routePath}. With Expired Bearer Token Returns 401 Unauthorized")]
	public Task Put_WithExpiredBearerToken_Returns_401Unauthorized()
		=> this.Action_WithExpiredBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new SubscriptionUpdateRequest()
			{
				Type = SubscriptionType.Normal,
				Status = SubscriptionStatus.Active
			};
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Sut.Client.PutAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions, 
				TestContext.Current.CancellationToken
			);
		});

	[Fact(DisplayName = $"PUT {routePath}. With Wrong Issuer Bearer Token Returns 401 Unauthorized")]
	public Task Put_WithWrongIssuerBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new SubscriptionUpdateRequest()
			{
				Type = SubscriptionType.Normal,
				Status = SubscriptionStatus.Active
			};
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Sut.Client.PutAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions, 
				TestContext.Current.CancellationToken
			);
		});


	[Fact(DisplayName = $"PUT {routePath}. With Wrong Audience Bearer Token Returns 401 Unauthorized")]
	public Task Put_WithWrongAudienceBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new SubscriptionUpdateRequest()
			{
				Type = SubscriptionType.Normal,
				Status = SubscriptionStatus.Active
			};
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Sut.Client.PutAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions, 
				TestContext.Current.CancellationToken
			);
		});


	[Fact(DisplayName = $"PUT {routePath}. With Wrongly Signed Bearer Token Returns 401 Unauthorized")]
	public Task Put_WithWronglySignedBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWronglySignedBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new SubscriptionUpdateRequest()
			{
				Type = SubscriptionType.Normal,
				Status = SubscriptionStatus.Active
			};
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Sut.Client.PutAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions, 
				TestContext.Current.CancellationToken
			);
		});

	#endregion

	#region 403 Forbidden

	[Fact(DisplayName = $"PUT {routePath}. Without Admin Bearer Token Returns 403 Forbidden")]
	public Task Put_WithoutAdminBearerToken_Returns_403Forbidden()
		=> this.Action_WithoutAdminBearerToken_Returns_403Forbidden(async () =>
		{
			var request = new SubscriptionUpdateRequest()
			{
				Type = SubscriptionType.Normal,
				Status = SubscriptionStatus.Active
			};
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Sut.Client.PutAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions, 
				TestContext.Current.CancellationToken
			);
		});

	#endregion

	#region 204 No Content

	[Fact(DisplayName = $"PUT {routePath}. With Valid Request Returns 204 No Content")]
	public async Task Put_WithValidRequest_Returns_204NoContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);
		var request = new SubscriptionUpdateRequest()
		{
			Type = SubscriptionType.Normal,
			Status = SubscriptionStatus.Active
		};
		var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

		var response = await this.Sut.Client.PutAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions, 
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
	}

	#endregion

	#region 404 Not Found

	[Fact(DisplayName = $"PUT {routePath}. For Non Existing Account Returns 404 Not Found")]
	public async Task Put_ForNonExistingAccount_Returns_404NotFound()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);

		var request = new SubscriptionUpdateRequest()
		{
			Type = SubscriptionType.Normal,
			Status = SubscriptionStatus.Active
		};
		var url = routePath.Replace("{id}", this.Sut.NonExistentId.ToString());

		var response = await this.Sut.Client.PutAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions, 
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
	}


	[Fact(DisplayName = $"PUT {routePath}. For Account Without Subscription Returns 404 Not Found")]
	public async Task Put_ForAccountWithoutSubscription_Returns_404NotFound()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);

		var request = new SubscriptionUpdateRequest()
		{
			Type = SubscriptionType.Normal,
			Status = SubscriptionStatus.Active
		};
		var url = routePath.Replace("{id}", this.Sut.ExistingAccountCredentials.Id.ToString());

		var response = await this.Sut.Client.PutAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions, 
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
	}

	#endregion

	#region 422 Unprocessable Content

	[Fact(DisplayName = $"PUT {routePath}. With Invalid Id Returns 422 Unprocessable Content")]
	public async Task Put_WithInvalidId_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);
		var request = new SubscriptionUpdateRequest()
		{
			Type = SubscriptionType.Normal,
			Status = SubscriptionStatus.Active
		};
		var url = routePath.Replace("{id}", "invalid");

		var response = await this.Sut.Client.PutAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions, 
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);
	}
	
	[Fact(DisplayName = $"PUT {routePath}. With Invalid Subscription Type Returns 422 Unprocessable Content")]
	public async Task Put_WithInvalidSubscriptionType_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);
		var request = new SubscriptionUpdateRequest
		{
			Type = null,
			Status = SubscriptionStatus.Active
		};
		var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

		var response = await this.Sut.Client.PutAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions, 
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);
	}
	
	[Fact(DisplayName = $"PUT {routePath}. With Invalid Subscription Status Returns 422 Unprocessable Content")]
	public async Task Put_WithInvalidSubscriptionStatus_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Sut.Admin);
		var request = new SubscriptionUpdateRequest
		{
			Type = SubscriptionType.Normal,
			Status = null
		};
		var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

		var response = await this.Sut.Client.PutAsJsonAsync(
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
		await this.Sut.EnsureAccountExistsWithSubscription(this.accountCredentialsUnderTest);
		await base.InitializeAsync();
	}

	public override async ValueTask DisposeAsync()
	{
		await this.Sut.EnsureAccountWithSubscriptionDoesNotExist(this.accountCredentialsUnderTest);
		await base.DisposeAsync();
	}
}
