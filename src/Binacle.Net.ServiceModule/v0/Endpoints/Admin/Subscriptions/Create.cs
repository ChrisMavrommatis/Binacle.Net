using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using FluxResults.TypedResults;
using FluxResults.Unions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin.Subscriptions;

internal class Create : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("/account/{id}/subscription", HandleAsync)
			.WithSummary("Create subscription")
			.WithDescription("Admins can use this endpoint to create a subscription for an account")
			.Accepts<SubscriptionCreateRequest>("application/json")
			.RequestExample<SubscriptionCreateRequestExample>("application/json")
			
			.Produces(StatusCodes.Status201Created)
			.ResponseDescription(StatusCodes.Status201Created, "The subscription for the specified account was created succesfully")
			
			.Produces(StatusCodes.Status404NotFound)
			.ResponseDescription(StatusCodes.Status404NotFound, SubscriptionResponseDescription.For404NotFound)
			
			.Produces(StatusCodes.Status409Conflict)
			.ResponseDescription(StatusCodes.Status409Conflict, SubscriptionResponseDescription.For409Conflict)
			
			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For422UnprocessableEntity
			)
			.ResponseExample<SubscriptionCreateValidationProblemExample>(
				StatusCodes.Status422UnprocessableEntity,
				"application/problem+json"
			);
	}

	internal async Task<IResult> HandleAsync(
		[AsParameters] AccountId id,
		AccountBindingResult<SubscriptionCreateRequest> bindingResult,
		IAccountRepository accountRepository,
		ISubscriptionRepository subscriptionRepository,
		TimeProvider timeProvider,
		CancellationToken cancellationToken = default)
	{
		return await bindingResult.ValidateAsync(id, async (request, account) =>
		{
			if (account.HasSubscription())
			{
				return Results.Conflict();
			}

			var utcNow = timeProvider.GetUtcNow();
			var subscription = new Subscription(
				account.Id,
				SubscriptionStatus.Active,
				request.Type!.Value,
				utcNow
			);

			var createResult = await subscriptionRepository.CreateAsync(subscription);
			if (!createResult.Is<Success>())
			{
				return TypedResults.Conflict();
			}

			account.SetSubscription(subscription);
			var updateAccountResult = await accountRepository.ForceUpdateAsync(account);
			
			if (!updateAccountResult.Is<Success>())
			{
				await subscriptionRepository.DeleteAsync(subscription);
				return Results.NotFound();
			}

			return Results.Created($"/api/admin/account/{account.Id}/subscription/{subscription.Id}", null);
		});
	}
}
