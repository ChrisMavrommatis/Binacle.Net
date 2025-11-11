using Binacle.Net.Configuration;
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
using Microsoft.Extensions.Options;
using OpenApiExamples;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v2.Endpoints.Pack;

internal class ByPreset : IGroupedEndpoint<ApiV2EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("pack/by-preset/{preset}", HandleAsync)
			.WithTags("Pack")
			.WithSummary("Pack by Preset")
			.WithDescription("Pack items using a specified bin preset.")
			.Accepts<PresetPackRequest>("application/json")
			.RequestExample<PresetPackRequestExample>("application/json")
			.Produces<PackResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseExamples<PresetPackResponseExamples>(StatusCodes.Status200OK, "application/json")
			.ResponseDescription(StatusCodes.Status200OK, ResponseDescription.ForPackResponse200Ok)
			.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.ResponseExamples<BadRequestErrorResponseExamples>(StatusCodes.Status400BadRequest, "application/json")
			.ResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.Produces(StatusCodes.Status404NotFound)
			.ResponseDescription(StatusCodes.Status404NotFound, ResponseDescription.ForPreset404NotFound)
			.RequireRateLimiting("ApiUsage")
			.RequireCors("Frontend");
	}

	internal async Task<IResult> HandleAsync(
		[FromRoute] string preset,
		LegacyBindingResult<PresetPackRequest> request,
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
			
			if (request.Value is null)
			{
				return Results.BadRequest(
					Response.ParameterError(
						nameof(request),
						ErrorMessage.MalformedRequestBody,
						ErrorCategory.RequestError
					)
				);
			}

			var validationResult = await validator.ValidateAsync(request.Value, cancellationToken);

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
				request.Value.Items!,
				new PackingParameters
				{
					Algorithm = Algorithm.FFD,
					StopAtSmallestBin = request.Value.Parameters?.StopAtSmallestBin ?? false,
					NeverReportUnpackedItems = request.Value.Parameters?.NeverReportUnpackedItems ?? false,
					OptInToEarlyFails = request.Value.Parameters?.OptInToEarlyFails ?? false,
					ReportPackedItemsOnlyWhenFullyPacked =
						request.Value.Parameters?.ReportPackedItemsOnlyWhenFullyPacked ?? false
				}
			);

			return Results.Ok(
				PackResponse.Create(
					presetOption.Bins,
					request.Value.Items!,
					request.Value.Parameters,
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
