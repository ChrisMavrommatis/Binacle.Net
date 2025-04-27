using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
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

internal class Delete : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapDelete("/account/{id}/subscription", HandleAsync)
			.WithSummary("Delete subscription")
			.WithDescription("Admins can use this endpoint to delete the subscription for an account")
			.Produces(StatusCodes.Status204NoContent)
			.WithResponseDescription(StatusCodes.Status204NoContent, DeleteSubscriptionResponseDescription.For204NoContent)
			.Produces(StatusCodes.Status400BadRequest)
			.ResponseExamples<DeleteSubscriptionErrorResponseExamples>(
				StatusCodes.Status400BadRequest,
				"application/json"
			)
			.Produces(StatusCodes.Status404NotFound)
			.WithResponseDescription(StatusCodes.Status404NotFound, SubscriptionResponseDescription.For404NotFound);
	}

	internal async Task<IResult> HandleAsync(
		string id,
		IAccountRepository accountRepository,
		ISubscriptionRepository subscriptionRepository,
		TimeProvider timeProvider,
		CancellationToken cancellationToken = default)
	{
		return await SubscriptionsRequestValidationExtensions
			.WithAccountValidatedTryCatch(accountRepository, id, async (account) =>
		{
			if (!account.HasSubscription())
			{
				return Results.NotFound();
			}

			var getResult = await subscriptionRepository.GetByIdAsync(account.SubscriptionId!.Value);
			if (!getResult.TryGetValue<Subscription>(out var subscription) || subscription is null)
			{
				return Results.NotFound();
			}

			var utcNow = timeProvider.GetUtcNow();
			subscription.SoftDelete(utcNow);
			var removeResult = account.RemoveSubscription();
			if (!removeResult.Is<Success>())
			{
				return Results.NotFound();
			}

			var updateAccountResult = await accountRepository.UpdateAsync(account);
			var updateSubscriptionResult = await subscriptionRepository.ForceUpdateAsync(subscription);

			if (!updateAccountResult.Is<Success>() || !updateSubscriptionResult.Is<Success>())
			{
				return Results.NotFound();
			}
			
			return Results.NoContent();

		});
	}
}
