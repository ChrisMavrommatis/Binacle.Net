using Binacle.Net.Api.ServiceModule.Configuration.Models;
using Binacle.Net.Api.ServiceModule.Domain.Users.Entities;
using Binacle.Net.Api.ServiceModule.Models;
using Binacle.Net.Api.ServiceModule.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;

namespace Binacle.Net.Api.ServiceModule.IntegrationTests.Abstractions;
public abstract partial class UsersEndpointTestsBase
{
	protected async Task Action_WithoutBearerToken_Returns_401Unauthorized(Func<Task<HttpResponseMessage>> action)
	{
		this.Sut.Client.DefaultRequestHeaders.Authorization = null;


		var response = await action();
		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
	}

	protected async Task Action_WithExpiredBearerToken_Returns_401Unauthorized(Func<Task<HttpResponseMessage>> action)
	{
		var timeProvider = new FakeTimeProvider();
		timeProvider.SetLocalTimeZone(TimeZoneInfo.Local);
		timeProvider.SetUtcNow(DateTime.UtcNow.AddHours(-10));

		var jwtAuthOptions = this.Sut.Services.GetService<IOptions<JwtAuthOptions>>();
		var tokenService = new TokenService(jwtAuthOptions!, timeProvider);

		var result = tokenService.GenerateStatelessToken(new StatelessTokenGenerationRequest(this.AdminUser.Email, UserGroups.Admins));

		this.Sut.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);

		var response = await action();

		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
	}

	protected async Task Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(Func<Task<HttpResponseMessage>> action)
	{
		var jwtAuthOptions = this.Sut.Services.GetService<IOptions<JwtAuthOptions>>()!;

		var newJwtAuthOptions = new JwtAuthOptions()
		{
			TokenSecret = jwtAuthOptions.Value.TokenSecret,
			ExpirationInSeconds = jwtAuthOptions.Value.ExpirationInSeconds,
			Audience = jwtAuthOptions.Value.Audience,
			Issuer = "WrongIssuer"
		};

		var tokenService = new TokenService(Options.Create(newJwtAuthOptions), TimeProvider.System);

		var result = tokenService.GenerateStatelessToken(new StatelessTokenGenerationRequest(this.AdminUser.Email, UserGroups.Admins));

		this.Sut.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);

		var response = await action();

		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
	}

	protected async Task Action_WithWrongAudienceBearerToken_Returns_401Unauthorized(Func<Task<HttpResponseMessage>> action)
	{
		var jwtAuthOptions = this.Sut.Services.GetService<IOptions<JwtAuthOptions>>()!;

		var newJwtAuthOptions = new JwtAuthOptions()
		{
			TokenSecret = jwtAuthOptions.Value.TokenSecret,
			ExpirationInSeconds = jwtAuthOptions.Value.ExpirationInSeconds,
			Audience = "WrongAudience",
			Issuer = jwtAuthOptions.Value.Issuer
		};

		var tokenService = new TokenService(Options.Create(newJwtAuthOptions), TimeProvider.System);

		var result = tokenService.GenerateStatelessToken(new StatelessTokenGenerationRequest(this.AdminUser.Email, UserGroups.Admins));

		this.Sut.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);

		var response = await action();

		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
	}

	protected async Task Action_WithWronglySignedBearerToken_Returns_401Unauthorized(Func<Task<HttpResponseMessage>> action)
	{
		var jwtAuthOptions = this.Sut.Services.GetService<IOptions<JwtAuthOptions>>()!;

		var newJwtAuthOptions = new JwtAuthOptions()
		{
			TokenSecret = "Bad" + jwtAuthOptions.Value.TokenSecret,
			ExpirationInSeconds = jwtAuthOptions.Value.ExpirationInSeconds,
			Audience = jwtAuthOptions.Value.Audience,
			Issuer = jwtAuthOptions.Value.Issuer
		};

		var tokenService = new TokenService(Options.Create(newJwtAuthOptions), TimeProvider.System);

		var result = tokenService.GenerateStatelessToken(new StatelessTokenGenerationRequest(this.AdminUser.Email, UserGroups.Admins));

		this.Sut.Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);

		var response = await action();

		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
	}

	protected async Task Action_WithoutAdminUserBearerToken_Returns_403Forbidden(Func<Task<HttpResponseMessage>> action)
	{
		await this.AuthenticateAsAsync(this.TestUser);

		var response = await action();

		response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Forbidden);
	}
}
