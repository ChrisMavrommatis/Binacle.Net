using System.Net.Mime;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin.Subscription;

internal class Get : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapGet("/account/{id}/subscription", HandleAsync)
			.WithSummary("Get subscription")
			.WithDescription("Admins can use this endpoint to get an account's subscription")
			.Produces<SubscriptionGetResponse>(StatusCodes.Status200OK)
			.ResponseDescription(StatusCodes.Status200OK, "The account has a subscription")
			.ResponseExample<SubscriptionGetResponseExample>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)

			.Produces(StatusCodes.Status404NotFound)
			.ResponseDescription(StatusCodes.Status404NotFound, SubscriptionResponseDescription.For404NotFound)

			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For422UnprocessableContent
			)
			.ResponseExample<SubscriptionGetValidationProblemExample>(
				StatusCodes.Status422UnprocessableEntity,
				MediaTypeNames.Application.ProblemJson
			);
	}

	internal static async Task<IResult> HandleAsync(
		[AsParameters] AccountId id,
		[FromQuery] bool? allowDeleted,
		IValidator<AccountId> validator,
		IAccountRepository accountRepository,
		ISubscriptionRepository subscriptionRepository,
		CancellationToken cancellationToken = default)
	{
		var validationResult = await validator.ValidateAsync(id, cancellationToken);
		if (!validationResult.IsValid)
		{
			return Results.ValidationProblem(
				validationResult.GetValidationSummary(),
				statusCode: StatusCodes.Status422UnprocessableEntity
			);
		}

		var accountResult = await accountRepository.GetByIdAsync(
			id.Value,
			allowDeleted ?? false,
			cancellationToken
		);

		if (!accountResult.TryGetValue<Domain.Accounts.Entities.Account>(out var account) || account is null)
		{
			return Results.NotFound();
		}

		if (!account.HasSubscription())
		{
			return Results.NotFound();
		}

		var subscriptionResult = await subscriptionRepository.GetByIdAsync(
			account.SubscriptionId!.Value,
			allowDeleted ?? false,
			cancellationToken
		);

		if (!subscriptionResult.TryGetValue<Domain.Subscriptions.Entities.Subscription>(out var subscription)
		    || subscription is null)
		{
			return Results.NotFound();
		}

		return Results.Ok(SubscriptionGetResponse.From(subscription));
	}
}
