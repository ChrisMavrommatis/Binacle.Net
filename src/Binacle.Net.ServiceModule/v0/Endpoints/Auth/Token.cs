using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Common.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.Services;
using Binacle.Net.ServiceModule.v0.Contracts.Auth;
using Binacle.Net.ServiceModule.v0.Resources;
using FluxResults.Unions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Auth;

internal class Token : IEndpoint
{
	public void DefineEndpoint(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapPost("/api/auth/token", HandleAsync)
			.WithTags("Auth")
			.WithSummary("Auth Token")
			.WithDescription(
				"Use this endpoint if you have the credentials to get a token and use the service without limits"
			)
			.Accepts<TokenRequest>("application/json")
			.RequestExample<TokenRequest.Example>("application/json")
			.Produces<TokenResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseExample<TokenResponse.Example>(StatusCodes.Status200OK, "application/json")
			.WithResponseDescription(StatusCodes.Status200OK, AuthTokenResponseDescription.For200OK)
			.Produces<AuthErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.ResponseExamples<AuthErrorResponse.Examples>(StatusCodes.Status400BadRequest, "application/json")
			.WithResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.Produces(StatusCodes.Status401Unauthorized)
			.WithResponseDescription(
				StatusCodes.Status401Unauthorized,
				AuthTokenResponseDescription.For401Unauthorized
			)
			.Produces(StatusCodes.Status403Forbidden)
			.WithResponseDescription(
				StatusCodes.Status403Forbidden,
				AuthTokenResponseDescription.For403Forbidden
			)
			.Produces<AuthErrorResponse>(StatusCodes.Status500InternalServerError, "application/json")
			.WithResponseDescription(
				StatusCodes.Status500InternalServerError,
				ResponseDescription.For500InternalServerError
			)
			.ResponseExample<AuthErrorResponse.InternalServerErrorExample>(
				StatusCodes.Status500InternalServerError,
				"application/json"
			)
			.RequireRateLimiting("Auth");
	}

	internal async Task<IResult> HandleAsync(
		BindingResult<TokenRequest> bindingResult,
		IAccountRepository accountRepository,
		ISubscriptionRepository subscriptionRepository,
		ITokenService tokenService,
		IPasswordService passwordService,
		CancellationToken cancellationToken = default
	)
	{
		return await bindingResult.ValidateAsync(async request =>
		{
			var accountResult = await accountRepository.GetByUsernameAsync(request.Username);
			if (!accountResult.TryGetValue<Account>(out var account) || account is null)
			{
				return Results.Unauthorized();
			}

			if (!account.IsActive() || !account.HasPassword())
			{
				return Results.Unauthorized();
			}

			if (!passwordService.PasswordMatches(account.Password!, request.Password))
			{
				return Results.Unauthorized();
			}

			Subscription? subscription = null;
			if (account.HasSubscription())
			{
				var subscriptionResult = await subscriptionRepository.GetByAccountIdAsync(account.Id);
				subscriptionResult.TryGetValue<Subscription>(out var foundSubscription);
				if (foundSubscription is not null && foundSubscription.IsActive())
				{
					subscription = foundSubscription;
				}
			}

			var tokenResult = tokenService.GenerateToken(account, subscription);

			return tokenResult.Match(
				token => Results.Ok(TokenResponse.Create(token)),
				ex => Results.InternalServerError(AuthErrorResponse.ServerError(ex))
			);
		});
	}
}
