using Binacle.Net.Api.Constants.Errors;
using Binacle.Net.Api.Kernel.Endpoints;
using Binacle.Net.Api.Models;
using Binacle.Net.Api.Services;
using Binacle.Net.Api.v2.Models;
using Binacle.Net.Api.v2.Requests;
using Binacle.Net.Api.v2.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.Api.v2.Endpoints.Fit;

internal class ByCustom : IGroupedEndpoint<ApiV2EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("fit/by-custom", HandleAsync)
			.WithTags("Fit")
			.WithSummary("Fit by custom")
			.WithDescription("Perform a bin fitting function using custom bins.")
			.Accepts<CustomFitRequest>("application/json")
			.Produces<FitResponse>(StatusCodes.Status200OK, "application/json")
			.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json")
			.WithOpenApi(operation =>
			{
				// Returns n array of results indicating if a bin can accommodate all the items
				// 	An array of results indicating if a bin can accommodate all the items.
				//	If the request is invalid.
				//	If an unexpected error occurs.
				// 
				// ///		Exception details will only be shown when in a development environment.
				return operation;
			});
		// [SwaggerRequestExample(typeof(v2.Requests.CustomFitRequest), typeof(v2.Requests.Examples.CustomFitRequestExample))]
		// [SwaggerResponseExample(typeof(v2.Responses.FitResponse), typeof(v2.Responses.Examples.CustomFitResponseExamples), StatusCodes.Status200OK)]
		// [SwaggerResponseExample(typeof(v2.Responses.ErrorResponse), typeof(v2.Responses.Examples.BadRequestErrorResponseExamples), StatusCodes.Status400BadRequest)]
		// [SwaggerResponseExample(typeof(v2.Responses.ErrorResponse), typeof(v2.Responses.Examples.ServerErrorResponseExample), StatusCodes.Status500InternalServerError)]

	}

	#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	internal async Task<IResult> HandleAsync(
		[FromBody] CustomFitRequest? request,
		IValidator<CustomFitRequest> validator,
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
					Response.ParameterError(
						nameof(request), Messages.MalformedRequestBody, Categories.RequestError)
				);
			}

			var validationResult = await validator.ValidateAsync(request, cancellationToken);

			if (!validationResult.IsValid)
			{
				return Results.BadRequest(
					Response.ValidationError(validationResult, Categories.ValidationError)
				);
			}

			var operationResults = await binsService.FitBinsAsync(
				request.Bins!, 
				request.Items!,
				new LegacyFittingParameters
				{
					FindSmallestBinOnly = request.Parameters?.FindSmallestBinOnly ?? true,
					ReportFittedItems = request.Parameters?.ReportFittedItems ?? false,
					ReportUnfittedItems = request.Parameters?.ReportUnfittedItems ?? false,
				}
			);

			return Results.Ok(
				FitResponse.Create(
					request.Bins!, 
					request.Items!, 
					request.Parameters,
					operationResults
				)
			);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An exception occurred in {endpoint} endpoint", "Query by Custom");
			return Results.InternalServerError(
				Response.ExceptionError(ex, Categories.ServerError)
			);
		}
	}
	
	
}
