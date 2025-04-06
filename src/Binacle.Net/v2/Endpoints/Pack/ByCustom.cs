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

namespace Binacle.Net.v2.Endpoints.Pack;

internal class ByCustom : IGroupedEndpoint<ApiV2EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("pack/by-custom", HandleAsync)
			.WithTags("Pack")
			.WithSummary("Pack by Custom")
			.WithDescription("Pack items using custom bins")
			.Accepts<CustomPackRequest>("application/json")
			.RequestExample<CustomPackRequestExample>("application/json")
			.Produces<PackResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseExamples<CustomPackResponseExamples>(StatusCodes.Status200OK, "application/json")
			.WithResponseDescription(StatusCodes.Status200OK, ResponseDescription.ForPackResponse200OK)
			.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.ResponseExamples<BadRequestErrorResponseExamples>(StatusCodes.Status400BadRequest, "application/json")
			.WithResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest);
	}
	
	internal async Task<IResult> HandleAsync(
		[FromBody] CustomPackRequest? request,
		IValidator<CustomPackRequest> validator,
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
						nameof(request),
						ErrorMessage.MalformedRequestBody, 
						ErrorCategory.RequestError
					)
				);
			}

			var validationResult = await validator.ValidateAsync(request, cancellationToken);

			if (!validationResult.IsValid)
			{
				return Results.BadRequest(
					Response.ValidationError(validationResult, ErrorCategory.ValidationError)
					);
			}

			var operationResults = await binsService.PackBinsAsync(
				request.Bins!,
				request.Items!,
				new LegacyPackingParameters
				{
					StopAtSmallestBin = request.Parameters?.StopAtSmallestBin ?? false,
					NeverReportUnpackedItems = request.Parameters?.NeverReportUnpackedItems ?? false,
					OptInToEarlyFails = request.Parameters?.OptInToEarlyFails ?? false,
					ReportPackedItemsOnlyWhenFullyPacked = request.Parameters?.ReportPackedItemsOnlyWhenFullyPacked ?? false
				}
			);

			return Results.Ok(
				PackResponse.Create(
					request.Bins!,
					request.Items!,
					request.Parameters,
					operationResults
				)
			);

		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An exception occurred in {endpoint} endpoint", "Pack by Custom");
			return Results.InternalServerError(
				Response.ExceptionError(ex, ErrorCategory.ServerError)
				);
		}
	}
}

