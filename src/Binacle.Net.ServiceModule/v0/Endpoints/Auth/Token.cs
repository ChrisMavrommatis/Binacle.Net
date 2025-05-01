using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Common.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.Services;
using Binacle.Net.ServiceModule.v0.Contracts.Auth;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
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
			.RequestExample<TokenRequestExample>("application/json")
			
			.Produces<TokenResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseExample<TokenResponseExample>(StatusCodes.Status200OK, "application/json")
			.ResponseDescription(StatusCodes.Status200OK, "The credentials are valid.")
			
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.ResponseExamples<Status400ResponseExamples>(StatusCodes.Status400BadRequest, "application/problem+json")
			
			.Produces(StatusCodes.Status401Unauthorized)
			.ResponseDescription(StatusCodes.Status401Unauthorized, "The credentials are invalid.")

			.Produces(StatusCodes.Status403Forbidden)
			.ResponseDescription(StatusCodes.Status403Forbidden, "The account is suspended.")
			
			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(StatusCodes.Status422UnprocessableEntity, ResponseDescription.For422UnprocessableEntity)
			.ResponseExample<TokenRequestValidationProblemExample>(
				StatusCodes.Status422UnprocessableEntity,
				"application/problem+json"
			)
			
			.ProducesProblem(StatusCodes.Status500InternalServerError)
			.ResponseDescription(StatusCodes.Status500InternalServerError, ResponseDescription.For500InternalServerError)
			.ResponseExample<Status500ResponseExample>(
				StatusCodes.Status500InternalServerError,
				"application/problem+json"
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

			if (!account.HasPassword())
			{
				return Results.Unauthorized();
			}

			if (!passwordService.PasswordMatches(account.Password!, request.Password))
			{
				return Results.Unauthorized();
			}
			
			if (account.IsSuspended())
			{
				return Results.Forbid();
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

			var token = tokenService.GenerateToken(account, subscription);
			return Results.Ok(TokenResponse.Create(token));
		});
	}
}
