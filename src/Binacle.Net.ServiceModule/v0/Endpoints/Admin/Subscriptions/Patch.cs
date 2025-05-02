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

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin.Subscriptions;

internal class Patch : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPatch("/account/{id}/subscription", HandleAsync)
			.WithSummary("Partiually update the subscription")
			.WithDescription("Admins can use this endpoint to partially update the subscription for an account")
			.Accepts<SubscriptionPatchRequest>("application/json")
			.RequestExamples<SubscriptionPatchRequestExamples>("application/json")

			.Produces(StatusCodes.Status204NoContent)
			.ResponseDescription(StatusCodes.Status204NoContent, "The subscription was updated succesfully")

			.Produces(StatusCodes.Status404NotFound)
			.ResponseDescription(StatusCodes.Status404NotFound, SubscriptionResponseDescription.For404NotFound)
			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For422UnprocessableEntity
			)
			.ResponseExample<SubscriptionPatchValidationProblemExample>(
				StatusCodes.Status422UnprocessableEntity,
				"application/problem+json"
			);
	}

	internal async Task<IResult> HandleAsync(
		[AsParameters] AccountId id,
		AccountBindingResult<SubscriptionPatchRequest> requestResult,
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
