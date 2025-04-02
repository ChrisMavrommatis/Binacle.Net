using Binacle.Net.Api.Constants.Errors;
using Binacle.Net.Api.Kernel.Endpoints;
using Binacle.Net.Api.Models;
using Binacle.Net.Api.Services;
using Binacle.Net.Api.v2.Models;
using Binacle.Net.Api.v2.Requests;
using Binacle.Net.Api.v2.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.Api.v2.Endpoints.Pack;

internal class ByCustom : IGroupedEndpoint<ApiV2EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("pack/by-custom", HandleAsync)
			.WithTags("Pack")
			.WithSummary("Pack by Custom")
			.WithDescription("Pack items using custom bins")
			.Accepts<CustomPackRequest>("application/json")
			.Produces<PackResponse>(StatusCodes.Status200OK, "application/json")
			.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json")
			.WithOpenApi(operation =>
			{
				// An array of results indicating the result per bin.
				//	If the request is invalid.
				//	If an unexpected error occurs.
				// 
				// ///		Exception details will only be shown when in a development environment.
				return operation;
			});
		// [SwaggerRequestExample(typeof(v2.Requests.CustomPackRequest), typeof(v2.Requests.Examples.CustomPackRequestExample))]
		// [SwaggerResponseExample(typeof(v2.Responses.PackResponse), typeof(v2.Responses.Examples.CustomPackResponseExamples), StatusCodes.Status200OK)]
		// [SwaggerResponseExample(typeof(v2.Responses.ErrorResponse), typeof(v2.Responses.Examples.BadRequestErrorResponseExamples), StatusCodes.Status400BadRequest)]
		// [SwaggerResponseExample(typeof(v2.Responses.ErrorResponse), typeof(v2.Responses.Examples.ServerErrorResponseExample), StatusCodes.Status500InternalServerError)]

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
						Messages.MalformedRequestBody, 
						Categories.RequestError
					)
				);
			}

			var validationResult = await validator.ValidateAsync(request, cancellationToken);

			if (!validationResult.IsValid)
			{
				return Results.BadRequest(
					Response.ValidationError(validationResult, Categories.ValidationError)
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
				Response.ExceptionError(ex, Categories.ServerError)
				);
		}
	}
}

