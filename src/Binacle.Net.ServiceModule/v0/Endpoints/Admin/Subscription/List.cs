using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OpenApiExamples;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.ServiceModule.v0.Endpoints.Admin.Subscription;

internal class List : IGroupedEndpoint<AdminGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapGet("/subscription/", HandleAsync)
			.WithSummary("List subscriptions")
			.WithDescription("Admins can use this endpoint to list all the subscriptions")
			.Produces<SubscriptionListResponse>(StatusCodes.Status200OK)
			.ResponseDescription(StatusCodes.Status200OK, "Lists the subscriptions with pagination")
			.ResponseExample<SubscriptionListResponseExample>(StatusCodes.Status200OK, "application/json")
			.Produces(StatusCodes.Status400BadRequest)
			
			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For422UnprocessableContent
			)
			.ResponseExample<SubscriptionListValidationProblemExample>(
				StatusCodes.Status422UnprocessableEntity,
				"application/problem+json"
			);
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
				validationResult!.GetValidationSummary(),
				statusCode: StatusCodes.Status422UnprocessableEntity
			);
		}

		var result = await subscriptionRepository.ListAsync(paging.PageNumber, paging.PageSize, cancellationToken);

		return Results.Ok(
			SubscriptionListResponse.Create(result)
		);
	}
}
