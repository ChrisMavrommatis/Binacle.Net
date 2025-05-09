using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using FluxResults.Unions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin.Subscription;

internal class Update : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPut("/account/{id}/subscription", HandleAsync)
			.WithSummary("Update subscription")
			.WithDescription("Admins can use this endpoint to update the subscription for an account")
			.Accepts<SubscriptionUpdateRequest>("application/json")
			.RequestExample<SubscriptionUpdateRequestExample>("application/json")
			
			.Produces(StatusCodes.Status204NoContent)
			.ResponseDescription(StatusCodes.Status204NoContent, "The subscription was updated successfully")

			.Produces(StatusCodes.Status404NotFound)
			.ResponseDescription(StatusCodes.Status404NotFound, SubscriptionResponseDescription.For404NotFound)
			
			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For422UnprocessableContent
			)
			.ResponseExamples<SubscriptionUpdateValidationProblemExamples>(
				StatusCodes.Status422UnprocessableEntity,
				"application/problem+json"
			);
	}

	internal async Task<IResult> HandleAsync(
		[AsParameters] AccountId id,
		AccountBindingResult<SubscriptionUpdateRequest> requestResult,
		ISubscriptionRepository subscriptionRepository,
		CancellationToken cancellationToken = default)
	{
		return await requestResult.ValidateAsync(id, async (request, account) =>
		{
			if (!account.HasSubscription())
			{
				return Results.NotFound();
			}

			var getResult = await subscriptionRepository.GetByIdAsync(account.SubscriptionId!.Value);
			if (!getResult.TryGetValue<Domain.Subscriptions.Entities.Subscription>(out var subscription) || subscription is null)
			{
				return Results.NotFound();
			}
			subscription.ChangeStatus(request.Status!.Value);
			subscription.ChangeType(request.Type!.Value);

			var updateResult = await subscriptionRepository.ForceUpdateAsync(subscription);

			return updateResult.Match(
				success => Results.NoContent(),
				notFound => Results.NotFound()
			);
		});
	}
}
