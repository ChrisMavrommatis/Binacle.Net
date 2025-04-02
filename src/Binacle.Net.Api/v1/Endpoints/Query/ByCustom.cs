using Binacle.Net.Api.Constants.Errors;
using Binacle.Net.Api.Kernel.Endpoints;
using Binacle.Net.Api.Models;
using Binacle.Net.Api.Services;
using Binacle.Net.Api.v1.Requests;
using Binacle.Net.Api.v1.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.Api.v1.Endpoints.Query;

internal class ByCustom : IGroupedEndpoint<ApiV1EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("query/by-custom", HandleAsync)
			.WithTags("Query")
			.WithSummary("Query by custom")
			.WithDescription("Perform a bin fit query using custom bins.")
			.Accepts<CustomQueryRequest>("application/json")
			.Produces<QueryResponse>(StatusCodes.Status200OK, "application/json")
			.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json")
			.WithOpenApi(operation =>
			{
				// Returns the bin that fits all of the items, or empty if they don't fit.
				// 	If the request is invalid.
				//	If an unexpected error occurs.
				//	Exception details will only be shown when in a development environment.
				return operation;
			});
		// [SwaggerRequestExample(typeof(v1.Requests.CustomQueryRequest), typeof(v1.Requests.Examples.CustomQueryRequestExample))]
		// [SwaggerResponseExample(typeof(v1.Responses.QueryResponse), typeof(v1.Responses.Examples.CustomQueryResponseExamples), StatusCodes.Status200OK)]
		// [SwaggerResponseExample(typeof(v1.Responses.ErrorResponse), typeof(v1.Responses.Examples.BadRequestErrorResponseExamples), StatusCodes.Status400BadRequest)]
		// [SwaggerResponseExample(typeof(v1.Responses.ErrorResponse), typeof(v1.Responses.Examples.ServerErrorResponseExample), StatusCodes.Status500InternalServerError)]
	}

	#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
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
					ErrorResponse.Create(Categories.RequestError)
						.AddParameterError(nameof(request), Messages.MalformedRequestBody)
				);
			}

			var validationResult = await validator.ValidateAsync(request, cancellationToken);
			

			if (!validationResult.IsValid)
			{
				return Results.BadRequest(
					ErrorResponse.Create(Categories.ValidationError)
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
				ErrorResponse.Create(Categories.ServerError)
					.AddExceptionError(ex)
			);
		}
	}
}
