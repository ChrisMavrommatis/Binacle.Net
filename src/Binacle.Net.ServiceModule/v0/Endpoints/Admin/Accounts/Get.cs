using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using FluentValidation;
using FluxResults.Unions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin.Accounts;

internal class Get : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapGet("/account/{id}", HandleAsync)
			.WithSummary("Get account")
			.WithDescription("Admins can use this endpoint to get an account's information")
			.Produces<AccountGetResponse>(StatusCodes.Status200OK)
			.ResponseDescription(StatusCodes.Status200OK, "The account exists")
			.ResponseExample<AccountGetResponseExample>(StatusCodes.Status200OK, "application/json")
			
			.Produces(StatusCodes.Status404NotFound)
			.ResponseDescription(StatusCodes.Status404NotFound, AccountResponseDescription.For404NotFound)
			
			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For422UnprocessableEntity
			)
			.ResponseExample<AccountGetValidationProblemExample>(
				StatusCodes.Status422UnprocessableEntity,
				"application/problem+json"
			);
	}

	internal async Task<IResult> HandleAsync(
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
				validationResult!.GetValidationSummary(),
				statusCode: StatusCodes.Status422UnprocessableEntity
			);
		}

		var accountResult = await accountRepository.GetByIdAsync(id.Value, allowDeleted ?? false);

		if (!accountResult.TryGetValue<Account>(out var account) || account is null)
		{
			return Results.NotFound();
		}

		if (!account.HasSubscription())
		{
			Results.Ok(
				AccountGetResponse.From(account)
			);
		}

		var subscriptionResult = await subscriptionRepository.GetByIdAsync(account.SubscriptionId!.Value,  allowDeleted ?? false);
		

		return subscriptionResult.Match(
			subscription => Results.Ok(
				AccountGetResponse.From(account, subscription)
			),
			notFound => Results.Ok(
				AccountGetResponse.From(account)
			)
		);
	}
}
