using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
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
			.Accepts<CreateSubscriptionRequest>("application/json")
			.RequestExample<CreateSubscriptionRequest.Example>("application/json")
			.Produces(StatusCodes.Status201Created)
			.WithResponseDescription(StatusCodes.Status201Created, CreateSubscriptionResponseDescription.For201Created)
			.ResponseExamples<CreateSubscriptionRequest.ErrorResponseExamples>(
				StatusCodes.Status400BadRequest,
				"application/json"
			)
			.Produces(StatusCodes.Status404NotFound)
			.WithResponseDescription(StatusCodes.Status404NotFound, SubscriptionResponseDescription.For404NotFound)
			.Produces(StatusCodes.Status409Conflict)
			.WithResponseDescription(StatusCodes.Status409Conflict, CreateSubscriptionResponseDescription.For409Conflict);
	}

	internal async Task<IResult> HandleAsync(
		string id,
		ValidatedBindingResult<CreateSubscriptionRequest> requestResult,
		IAccountRepository accountRepository,
		ISubscriptionRepository subscriptionRepository,
		TimeProvider timeProvider,
		CancellationToken cancellationToken = default)
	{
		return await requestResult.WithAccountValidatedRequest(accountRepository, id, async (request, account) =>
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
