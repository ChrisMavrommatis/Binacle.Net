using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin.Subscription;

internal class Delete : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapDelete("/account/{id}/subscription", HandleAsync)
			.WithSummary("Delete subscription")
			.WithDescription("Admins can use this endpoint to delete the subscription for an account")
			.Produces(StatusCodes.Status204NoContent)
			.ResponseDescription(StatusCodes.Status204NoContent, "The Subscription was deleted")
			
			.Produces(StatusCodes.Status404NotFound)
			.ResponseDescription(StatusCodes.Status404NotFound, SubscriptionResponseDescription.For404NotFound)
			
			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For422UnprocessableContent
			)
			.ResponseExample<SubscriptionDeleteValidationProblemExample>(
				StatusCodes.Status422UnprocessableEntity,
				"application/problem+json"
			);
	}

	internal async Task<IResult> HandleAsync(
		[AsParameters] AccountId id,
		IValidator<AccountId> validator,
		IAccountRepository accountRepository,
		ISubscriptionRepository subscriptionRepository,
		TimeProvider timeProvider,
		CancellationToken cancellationToken = default)
	{
		var validationResult = await validator.ValidateAsync(id, cancellationToken);
		if (!validationResult.IsValid)
		{
			return Results.ValidationProblem(
				validationResult!.GetValidationSummary(),
				statusCode: StatusCodes.Status422UnprocessableEntity
			);
		}

		var accountResult = await accountRepository.GetByIdAsync(id.Value, cancellationToken: cancellationToken);
		if (!accountResult.TryGetValue<Domain.Accounts.Entities.Account>(out var account) || account is null)
		{
			return Results.NotFound();
		}

		if (!account.HasSubscription())
		{
			return Results.NotFound();
		}

		var getResult = await subscriptionRepository.GetByIdAsync(
			account.SubscriptionId!.Value,
			cancellationToken: cancellationToken
		);
		
		if (!getResult.TryGetValue<Domain.Subscriptions.Entities.Subscription>(out var subscription) ||
		    subscription is null)
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

		var updateAccountResult = await accountRepository.UpdateAsync(account, cancellationToken);
		var updateSubscriptionResult = await subscriptionRepository.UpdateAsync(subscription, cancellationToken);

		if (!updateAccountResult.Is<Success>() || !updateSubscriptionResult.Is<Success>())
		{
			return Results.NotFound();
		}

		return Results.NoContent();
	}
}
