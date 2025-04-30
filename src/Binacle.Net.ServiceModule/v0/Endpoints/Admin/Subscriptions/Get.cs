using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
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

internal class Get : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapGet("/account/{id}/subscription", HandleAsync)
			.WithSummary("Get subscription")
			.WithDescription("Admins can use this endpoint to get the subscription for an account")
			.Produces<GetSubscriptionResponse>(StatusCodes.Status200OK)
			.ResponseDescription(StatusCodes.Status200OK, GetSubscriptionResponseDescription.For200OK)
			.Produces<GetSubscriptionResponse>(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status400BadRequest)
			.ResponseExample<GetSubscriptionResponse.ErrorResponseExample>(
				StatusCodes.Status400BadRequest,
				"application/json"
			)
			.Produces(StatusCodes.Status404NotFound)
			.ResponseDescription(StatusCodes.Status404NotFound, SubscriptionResponseDescription.For404NotFound);
	}

	internal async Task<IResult> HandleAsync(
		string id,
		IAccountRepository accountRepository,
		ISubscriptionRepository subscriptionRepository,
		TimeProvider timeProvider,
		CancellationToken cancellationToken = default)
	{
		if (!Guid.TryParse(id, out var accountId))
		{
			return Results.BadRequest(
				ErrorResponse.IdToGuidParameterError
			);
		}

		var accountResult = await accountRepository.GetByIdAsync(accountId);
		if (!accountResult.TryGetValue<Account>(out var account) || account is null)
		{
			return Results.NotFound();
		}

		if (!account.HasSubscription())
		{
			return Results.NotFound();
		}

		var getResult = await subscriptionRepository.GetByIdAsync(account.SubscriptionId!.Value);
		return getResult.Match(
			subscription => Results.Ok(
				GetSubscriptionResponse.From(subscription)
			),
			notFound => Results.NotFound()
		);
	}
}
