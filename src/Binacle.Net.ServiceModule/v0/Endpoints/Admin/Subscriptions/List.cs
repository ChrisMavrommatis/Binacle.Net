using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using FluentValidation;
using FluxResults.TypedResults;
using FluxResults.Unions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin.Subscriptions;

internal class List : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapGet("/subscription/", HandleAsync)
			.WithSummary("List subscriptions")
			.WithDescription("Admins can use this endpoint to list all the subscriptions")
			.Produces<ListSubscriptionsResponse>(StatusCodes.Status200OK)
			.WithResponseDescription(StatusCodes.Status200OK, ListSubscriptionResponseDescription.For200OK)
			.ResponseExample<ListSubscriptionsResponse.Example>(StatusCodes.Status200OK, "application/json")
			.Produces(StatusCodes.Status400BadRequest)
			.ResponseExamples<ListSubscriptionsResponse.ErrorResponseExamples>(
				StatusCodes.Status400BadRequest,
				"application/json"
			)
			.Produces(StatusCodes.Status404NotFound)
			.WithResponseDescription(StatusCodes.Status404NotFound, ListSubscriptionResponseDescription.For404NotFound);
	}

	internal async Task<IResult> HandleAsync(
		[AsParameters] PagingQuery paging,
		IValidator<PagingQuery> validator,
		ISubscriptionRepository subscriptionRepository,
		CancellationToken cancellationToken = default)
	{
		var validationResult = await validator.ValidateAsync(paging, cancellationToken);
		if (!validationResult.IsValid)
		{
			return Results.ValidationProblem(
				validationResult!.GetValidationSummary()
			);
		}

		var result = await subscriptionRepository.ListAsync(paging.PageNumber, paging.PageSize);

		return result.Match(
			subscriptions => Results.Ok(
				ListSubscriptionsResponse.From(subscriptions)
			),
			notFound => Results.NotFound()
		);
	}
}
