using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Resources;
using FluxResults.Unions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin.Subscriptions;

internal class Patch : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPatch("/account/{id}/subscription", HandleAsync)
			.WithSummary("Partiually update the subscription")
			.WithDescription("Admins can use this endpoint to partially update the subscription for an account")
			.Accepts<PartialUpdateSubscriptionRequest>("application/json")
			.RequestExamples<PartialUpdateSubscriptionRequest.Examples>("application/json")
			.Produces(StatusCodes.Status204NoContent)
			.WithResponseDescription(StatusCodes.Status204NoContent,
				UpdateSubscriptionResponseDescription.For204NoContent)
			.ResponseExamples<PartialUpdateSubscriptionRequest.ErrorResponseExamples>(
				StatusCodes.Status400BadRequest,
				"application/json"
			)
			.Produces(StatusCodes.Status404NotFound)
			.WithResponseDescription(StatusCodes.Status404NotFound, SubscriptionResponseDescription.For404NotFound);
	}

	internal async Task<IResult> HandleAsync(
		string id,
		AccountBindingResult<PartialUpdateSubscriptionRequest> requestResult,
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
			if (!getResult.TryGetValue<Subscription>(out var subscription) || subscription is null)
			{
				return Results.NotFound();
			}

			if (request.Status.HasValue)
			{
				subscription.ChangeStatus(request.Status.Value);
			}
			
			if (request.Type.HasValue)
			{
				subscription.ChangeType(request.Type.Value);
			}

			var updateResult = await subscriptionRepository.ForceUpdateAsync(subscription);

			return updateResult.Match(
				success => Results.NoContent(),
				notFound => Results.NotFound()
			);
		});
	}
}
