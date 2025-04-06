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
			.WithResponseDescription(StatusCodes.Status200OK, ResponseDescription.ForFitResponse200OK)
			.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.ResponseExamples<BadRequestErrorResponseExamples>(StatusCodes.Status400BadRequest, "application/json")
			.WithResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest);
	}

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
						nameof(request), ErrorMessage.MalformedRequestBody, ErrorCategory.RequestError)
				);
			}

			var validationResult = await validator.ValidateAsync(request, cancellationToken);

			if (!validationResult.IsValid)
			{
				return Results.BadRequest(
					Response.ValidationError(validationResult, ErrorCategory.ValidationError)
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
				Response.ExceptionError(ex, ErrorCategory.ServerError)
			);
		}
	}
	
	
}
