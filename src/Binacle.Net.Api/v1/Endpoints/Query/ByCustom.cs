﻿using Binacle.Net.Api.Constants;
using Binacle.Net.Api.Kernel.Endpoints;
using Binacle.Net.Api.Models;
using Binacle.Net.Api.Services;
using Binacle.Net.Api.v1.Requests;
using Binacle.Net.Api.v1.Requests.Examples;
using Binacle.Net.Api.v1.Responses;
using Binacle.Net.Api.v1.Responses.Examples;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OpenApiExamples;

namespace Binacle.Net.Api.v1.Endpoints.Query;

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
			.WithResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest);
	}

	internal async Task<IResult> HandleAsync(
		[FromBody] CustomQueryRequest? request,
		IValidator<CustomQueryRequest> validator,
		ILegacyBinsService binsService,
		ILogger<ByCustom> logger,
		CancellationToken cancellationToken = default
	)
	{
		try
		{
			if (request is null)
			{
				return Results.BadRequest(
					ErrorResponse.Create(ErrorCategory.RequestError)
						.AddParameterError(nameof(request), ErrorMessage.MalformedRequestBody)
				);
			}

			var validationResult = await validator.ValidateAsync(request, cancellationToken);
			

			if (!validationResult.IsValid)
			{
				return Results.BadRequest(
					ErrorResponse.Create(ErrorCategory.ValidationError)
						.AddValidationResult(validationResult)
				);
			}

			var operationResults = await binsService.FitBinsAsync(
				request.Bins!, 
				request.Items!,
				new LegacyFittingParameters
				{
					FindSmallestBinOnly = true,
					ReportFittedItems = false,
					ReportUnfittedItems = false
				}
			);

			return Results.Ok(
				QueryResponse.Create(request.Bins!, request.Items!, operationResults)
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
