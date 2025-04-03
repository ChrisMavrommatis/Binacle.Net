using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.Api.Constants;
using Binacle.Net.Api.Kernel.Endpoints;
using Binacle.Net.Api.Models;
using Binacle.Net.Api.Services;
using Binacle.Net.Api.v2.Models;
using Binacle.Net.Api.v2.Requests;
using Binacle.Net.Api.v2.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Binacle.Net.Api.v2.Endpoints.Pack;

internal class ByPreset : IGroupedEndpoint<ApiV2EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("pack/by-preset/{preset}", HandleAsync)
			.WithTags("Pack")
			.WithSummary("Pack by Preset")
			.WithDescription("Pack items using a specified bin preset.")
			.Accepts<PresetPackRequest>("application/json")
			.Produces<PackResponse>(StatusCodes.Status200OK, "application/json")
			.WithResponseDescription(StatusCodes.Status200OK, ResponseDescription.ForPackResponse200OK)
			.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.WithResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.Produces(StatusCodes.Status404NotFound)
			.WithResponseDescription(StatusCodes.Status404NotFound, ResponseDescription.ForPreset404NotFound);

		// [SwaggerRequestExample(typeof(v2.Requests.PresetPackRequest), typeof(v2.Requests.Examples.PresetPackRequestExample))]
		// [SwaggerResponseExample(typeof(v2.Responses.PackResponse), typeof(v2.Responses.Examples.PresetPackResponseExamples), StatusCodes.Status200OK)]
		// [SwaggerResponseExample(typeof(v2.Responses.ErrorResponse), typeof(v2.Responses.Examples.BadRequestErrorResponseExamples), StatusCodes.Status400BadRequest)]
		// [SwaggerResponseExample(typeof(v2.Responses.ErrorResponse), typeof(v2.Responses.Examples.ServerErrorResponseExample), StatusCodes.Status500InternalServerError)]
	}

	internal async Task<IResult> HandleAsync(
		[FromRoute] string preset,
		[FromBody] PresetPackRequest? request,
		IValidator<PresetPackRequest> validator,
		IOptions<BinPresetOptions> presetOptions,
		ILegacyBinsService binsService,
		ILogger<ByPreset> logger,
		CancellationToken cancellationToken = default
	)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(preset))
			{
				return Results.BadRequest(
					Response.ParameterError(
						nameof(preset), 
						ErrorMessage.IsRequired, 
						ErrorCategory.RequestError
					)
				);
			}
			
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

			if (!presetOptions.Value.Presets.TryGetValue(preset, out var presetOption))
			{
				return Results.NotFound(null);
			}

			var operationResults = await binsService.PackBinsAsync(
				presetOption.Bins,
				request.Items!,
				new LegacyPackingParameters
				{
					StopAtSmallestBin = request.Parameters?.StopAtSmallestBin ?? false,
					NeverReportUnpackedItems = request.Parameters?.NeverReportUnpackedItems ?? false,
					OptInToEarlyFails = request.Parameters?.OptInToEarlyFails ?? false,
					ReportPackedItemsOnlyWhenFullyPacked =
						request.Parameters?.ReportPackedItemsOnlyWhenFullyPacked ?? false
				}
			);

			return Results.Ok(
				PackResponse.Create(
					presetOption.Bins,
					request.Items!,
					request.Parameters,
					operationResults
				)
			);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An exception occurred in {endpoint} endpoint", "Pack by Preset");
			return Results.InternalServerError(
				Response.ExceptionError(ex, ErrorCategory.ServerError)
			);
		}
	}
}
