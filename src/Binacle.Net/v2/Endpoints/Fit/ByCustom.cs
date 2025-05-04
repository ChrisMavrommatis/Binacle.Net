using Binacle.Net.Constants;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.Models;
using Binacle.Net.Services;
using Binacle.Net.v2.Models;
using Binacle.Net.v2.Requests;
using Binacle.Net.v2.Requests.Examples;
using Binacle.Net.v2.Responses;
using Binacle.Net.v2.Responses.Examples;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OpenApiExamples;

namespace Binacle.Net.v2.Endpoints.Fit;

internal class ByCustom : IGroupedEndpoint<ApiV2EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("fit/by-custom", HandleAsync)
			.WithTags("Fit")
			.WithSummary("Fit by Custom")
			.WithDescription("Perform a bin fitting function using custom bins.")
			.Accepts<CustomFitRequest>("application/json")
			.RequestExample<CustomFitRequestExample>("application/json")
			.Produces<FitResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseExamples<CustomFitResponseExamples>(StatusCodes.Status200OK, "application/json")
			.ResponseDescription(StatusCodes.Status200OK, ResponseDescription.ForFitResponse200OK)
			.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.ResponseExamples<BadRequestErrorResponseExamples>(StatusCodes.Status400BadRequest, "application/json")
			.ResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.RequireRateLimiting("ApiUsage");
	}

	internal async Task<IResult> HandleAsync(
		LegacyBindingResult<CustomFitRequest> request,
		IValidator<CustomFitRequest> validator,
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
					Response.ParameterError(
						nameof(request), ErrorMessage.MalformedRequestBody, ErrorCategory.RequestError)
				);
			}

			var validationResult = await validator.ValidateAsync(request.Value, cancellationToken);

			if (!validationResult.IsValid)
			{
				return Results.BadRequest(
					Response.ValidationError(validationResult, ErrorCategory.ValidationError)
				);
			}

			var operationResults = await binsService.FitBinsAsync(
				request.Value.Bins!, 
				request.Value.Items!,
				new LegacyFittingParameters
				{
					FindSmallestBinOnly = request.Value.Parameters?.FindSmallestBinOnly ?? true,
					ReportFittedItems = request.Value.Parameters?.ReportFittedItems ?? false,
					ReportUnfittedItems = request.Value.Parameters?.ReportUnfittedItems ?? false,
				}
			);

			return Results.Ok(
				FitResponse.Create(
					request.Value.Bins!, 
					request.Value.Items!, 
					request.Value.Parameters,
					operationResults
				)
			);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An exception occurred in {endpoint} endpoint", "Query by Custom");
			return Results.InternalServerError(
				Response.ExceptionError(ex, ErrorCategory.ServerError)
			);
		}
	}
	
	
}
