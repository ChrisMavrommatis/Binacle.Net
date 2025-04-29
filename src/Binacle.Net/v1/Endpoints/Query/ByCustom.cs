using Binacle.Net.Constants;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.Models;
using Binacle.Net.Services;
using Binacle.Net.v1.Requests;
using Binacle.Net.v1.Requests.Examples;
using Binacle.Net.v1.Responses;
using Binacle.Net.v1.Responses.Examples;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OpenApiExamples;

namespace Binacle.Net.v1.Endpoints.Query;

internal class ByCustom : IGroupedEndpoint<ApiV1EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("query/by-custom", HandleAsync)
			.WithTags("Query")
			.WithSummary("Query by Custom")
			.WithDescription("Perform a bin fit query using custom bins.")
			.Accepts<CustomQueryRequest>("application/json")
			.RequestExample<CustomQueryRequestExample>("application/json")
			.Produces<QueryResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseExamples<CustomQueryResponseExamples>(StatusCodes.Status200OK, "application/json")
			.WithResponseDescription(StatusCodes.Status200OK, ResponseDescription.ForQueryResponse200OK)
			.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.ResponseExamples<BadRequestErrorResponseExamples>(StatusCodes.Status400BadRequest, "application/json")
			.WithResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.RequireRateLimiting("Anonymous");
	}

	internal async Task<IResult> HandleAsync(
		LegacyBindingResult<CustomQueryRequest> request,
		IValidator<CustomQueryRequest> validator,
		ILegacyBinsService binsService,
		ILogger<ByCustom> logger,
		CancellationToken cancellationToken = default
	)
	{
		try
		{
			if (request.Value is null)
			{
				return Results.BadRequest(
					ErrorResponse.Create(ErrorCategory.RequestError)
						.AddParameterError(nameof(request), ErrorMessage.MalformedRequestBody)
				);
			}

			var validationResult = await validator.ValidateAsync(request.Value, cancellationToken);
			

			if (!validationResult.IsValid)
			{
				return Results.BadRequest(
					ErrorResponse.Create(ErrorCategory.ValidationError)
						.AddValidationResult(validationResult)
				);
			}

			var operationResults = await binsService.FitBinsAsync(
				request.Value.Bins!, 
				request.Value.Items!,
				new LegacyFittingParameters
				{
					FindSmallestBinOnly = true,
					ReportFittedItems = false,
					ReportUnfittedItems = false
				}
			);

			return Results.Ok(
				QueryResponse.Create(request.Value.Bins!, request.Value.Items!, operationResults)
			);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An exception occurred in {endpoint} endpoint", "Query by Custom");
			return Results.InternalServerError(
				ErrorResponse.Create(ErrorCategory.ServerError)
					.AddExceptionError(ex)
			);
		}
	}
}
