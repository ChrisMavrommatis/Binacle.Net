// using Binacle.Net.ServiceModule.IntegrationTests.Models;
// using System.Net.Http.Json;
//
// namespace Binacle.Net.ServiceModule.IntegrationTests;
//
// [Trait("Endpoint Tests", "Endpoint Integration tests")]
// [Collection(BinacleApiAsAServiceCollection.Name)]
// public class ChangeUserPassword : Abstractions.UsersEndpointTestsBase
// {
// 	private readonly TestUser existingUser;
//
// 	public ChangeUserPassword(BinacleApiAsAServiceFactory sut) : base(sut)
// 	{
// 		this.existingUser = new TestUser()
// 		{
// 			Email = "existing@user.test",
// 			Password = "Ex1stingUs3rP@ssw0rd"
//
// 		};
// 	}
//
// 	private const string routePath = "/api/users/{email}";
//
// 	#region 401 Unauthorized
//
// 	[Fact(DisplayName = $"PATCH {routePath}. Without Bearer Token Returns 401 Unauthorized")]
// 	public Task Patch_WithoutBearerToken_Returns_401Unauthorized()
// 		=> this.Action_WithoutBearerToken_Returns_401Unauthorized(async () =>
// 		{
// 			var url = routePath.Replace("{email}", this.existingUser.Email);
// 			var request = new Net.ServiceModule.v0.Contracts.Auth.ChangeApiUserPasswordRequest
// 			{
// 				Password = "Ex1stingUs3rNewP@ssw0rd"
// 			};
// 			return await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
// 		});
//
// 	[Fact(DisplayName = $"PATCH {routePath}. With Expired Bearer Token Returns 401 Unauthorized")]
// 	public Task Patch_WithExpiredBearerToken_Returns_401Unauthorized()
// 		=> this.Action_WithExpiredBearerToken_Returns_401Unauthorized(async () =>
// 		{
// 			var url = routePath.Replace("{email}", this.existingUser.Email);
// 			var request = new Net.ServiceModule.v0.Requests.ChangeApiUserPasswordRequest
// 			{
// 				Password = "Ex1stingUs3rNewP@ssw0rd"
// 			};
// 			return await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
// 		});
//
//
// 	[Fact(DisplayName = $"PATCH {routePath}. With Wrong Issuer Bearer Token Returns 401 Unauthorized")]
// 	public Task Patch_WithWrongIssuerBearerToken_Returns_401Unauthorized()
// 		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
// 		{
// 			var url = routePath.Replace("{email}", this.existingUser.Email);
// 			var request = new Net.ServiceModule.v0.Requests.ChangeApiUserPasswordRequest
// 			{
// 				Password = "Ex1stingUs3rNewP@ssw0rd"
// 			};
// 			return await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
// 		});
//
// 	[Fact(DisplayName = $"PATCH {routePath}. With Wrong Audience Bearer Token Returns 401 Unauthorized")]
// 	public Task Patch_WithWrongAudienceBearerToken_Returns_401Unauthorized()
// 		=> this.Action_WithWrongAudienceBearerToken_Returns_401Unauthorized(async () =>
// 		{
// 			var url = routePath.Replace("{email}", this.existingUser.Email);
// 			var request = new Net.ServiceModule.v0.Requests.ChangeApiUserPasswordRequest
// 			{
// 				Password = "Ex1stingUs3rNewP@ssw0rd"
// 			};
// 			return await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
// 		});
//
// 	[Fact(DisplayName = $"PATCH {routePath}. With Wrongly Signed Bearer Token Returns 401 Unauthorized")]
// 	public Task Patch_WithWronglySignedBearerToken_Returns_401Unauthorized()
// 		=> this.Action_WithWronglySignedBearerToken_Returns_401Unauthorized(async () =>
// 		{
// 			var url = routePath.Replace("{email}", this.existingUser.Email);
// 			var request = new Net.ServiceModule.v0.Requests.ChangeApiUserPasswordRequest
// 			{
// 				Password = "Ex1stingUs3rNewP@ssw0rd"
// 			};
// 			return await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
// 		});
//
// 	#endregion
//
// 	#region 403 Forbidden
//
// 	[Fact(DisplayName = $"PATCH {routePath}. Without Admin User Bearer Token Returns 403 Forbidden")]
// 	public Task Patch_WithoutAdminUserBearerToken_Returns_403Forbidden()
// 		=> this.Action_WithoutAdminUserBearerToken_Returns_403Forbidden(async () =>
// 		{
// 			var url = routePath.Replace("{email}", this.existingUser.Email);
// 			var request = new Net.ServiceModule.v0.Requests.ChangeApiUserPasswordRequest
// 			{
// 				Password = "Ex1stingUs3rNewP@ssw0rd"
// 			};
// 			return await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
// 		});
//
// 	#endregion
//
// 	#region 204 No Content
//
// 	[Fact(DisplayName = $"PATCH {routePath}. With Valid Credentials Returns 204 No Content")]
// 	public async Task Patch_WithValidCredentials_Returns_204NoContent()
// 	{
// 		await this.AuthenticateAsAsync(this.AdminUser);
//
// 		var url = routePath.Replace("{email}", this.existingUser.Email);
// 		var request = new Net.ServiceModule.v0.Requests.ChangeApiUserPasswordRequest
// 		{
// 			Password = "Ex1stingUs3rNewP@ssw0rd"
// 		};
// 		var response = await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
// 		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
// 	}
// 	#endregion
//
// 	#region 400 Bad Request
//
// 	[Fact(DisplayName = $"PATCH {routePath}. With Invalid Email Returns 400 BadRequest")]
// 	public async Task Patch_WithInvalidEmail_Returns_400BadRequest()
// 	{
// 		await this.AuthenticateAsAsync(this.AdminUser);
//
// 		var url = routePath.Replace("{email}", "existinguser.test");
//
// 		var request = new Net.ServiceModule.v0.Requests.ChangeApiUserPasswordRequest
// 		{
// 			Password = this.existingUser.Password
// 		};
// 		var response = await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
// 		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
// 	}
// 	
// 	[Fact(DisplayName = $"PATCH {routePath}. With Invalid Password Returns 400 BadRequest")]
// 	public async Task Patch_WithInvalidPassword_Returns_400BadRequest()
// 	{
// 		await this.AuthenticateAsAsync(this.AdminUser);
//
// 		var url = routePath.Replace("{email}", this.existingUser.Email);
//
// 		var request = new Net.ServiceModule.v0.Requests.ChangeApiUserPasswordRequest
// 		{
// 			Password = "password"
// 		};
// 		var response = await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
// 		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
// 	}
//
// 	#endregion
//
// 	#region 404 Not Found
//
// 	[Fact(DisplayName = $"PATCH {routePath}. For Non Existing User Returns 404 Not Found")]
// 	public async Task Patch_ForNonExistingUser_Returns_404NotFound()
// 	{
// 		await this.AuthenticateAsAsync(this.AdminUser);
//
// 		var url = routePath.Replace("{email}", "nonexisting@user.test");
// 		var request = new Net.ServiceModule.v0.Requests.ChangeApiUserPasswordRequest
// 		{
// 			Password = "Ex1stingUs3rNewP@ssw0rd"
// 		};
// 		var response = await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
// 		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
// 	}
//
// 	#endregion
//
// 	#region 409 Conflict
//
// 	[Fact(DisplayName = $"PATCH {routePath}. With Same Password Returns 409 Conflict")]
// 	public async Task Patch_WithSamePassword_Returns_409Conflict()
// 	{
// 		await this.AuthenticateAsAsync(this.AdminUser);
//
// 		var url = routePath.Replace("{email}", this.existingUser.Email);
//
// 		var request = new Net.ServiceModule.v0.Requests.ChangeApiUserPasswordRequest
// 		{
// 			Password = this.existingUser.Password
// 		};
// 		var response = await this.Sut.Client.PatchAsJsonAsync(url, request, this.Sut.JsonSerializerOptions);
// 		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Conflict);
// 	}
//
// 	#endregion
//
// 	public override async Task InitializeAsync()
// 	{
// 		await this.CreateUser(this.existingUser);
// 		await base.InitializeAsync();
// 	}
//
// 	public override async Task DisposeAsync()
// 	{
// 		await this.DeleteUser(this.existingUser);
// 		await base.DisposeAsync();
// 	}
// }
