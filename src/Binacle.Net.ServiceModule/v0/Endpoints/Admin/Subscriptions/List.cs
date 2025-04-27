using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Entities;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using Binacle.Net.ServiceModule.v0.Resources;
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
		int? pg,
		int? pz,
		ISubscriptionRepository subscriptionRepository,
		CancellationToken cancellationToken = default)
	{
		return await RequestValidationExtensions.WithTryCatch(async () =>
		{
			if (pg is < 1)
			{
				return Results.BadRequest(
					ErrorResponse.PageNumberError
				);
			}
		
			if (pz is < 1)
			{
				return Results.BadRequest(
					ErrorResponse.PageSizeError
				);
			}
			var result = await subscriptionRepository.GetAsync(pg ?? 1, pz ?? 10);

			return result.Match(
				subscriptions => Results.Ok(
					ListSubscriptionsResponse.From(subscriptions)
				),
				notFound => Results.NotFound()
			);
		});
	}
}
