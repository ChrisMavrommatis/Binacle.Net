using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.Api.Constants.Errors;
using Binacle.Net.Api.Kernel.Endpoints;
using Binacle.Net.Api.Models;
using Binacle.Net.Api.Services;
using Binacle.Net.Api.v1.Requests;
using Binacle.Net.Api.v1.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Binacle.Net.Api.v1.Endpoints.Query;

internal class ByPreset : IGroupedEndpoint<ApiV1EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("query/by-preset/{preset}", HandleAsync)
			.WithTags("Query")
			.WithSummary("Query by Preset")
			.WithDescription("Perform a bin fit query using a specified bin preset")
			.Accepts<PresetQueryRequest>("application/json")
			.Produces<QueryResponse>(StatusCodes.Status200OK, "application/json")
			.Produces(StatusCodes.Status404NotFound)
			.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.Produces<ErrorResponse>(StatusCodes.Status500InternalServerError, "application/json")
			.WithOpenApi(operation =>
			{
				// Returns the bin that fits all of the items, or empty if they don't fit.
				// 	If the request is invalid.
				// If the preset does not exist
				//	If an unexpected error occurs.
				//	Exception details will only be shown when in a development environment.
				return operation;
			});
		// [SwaggerRequestExample(typeof(v1.Requests.PresetQueryRequest), typeof(v1.Requests.Examples.PresetQueryRequestExample))]
		// [SwaggerResponseExample(typeof(v1.Responses.QueryResponse), typeof(v1.Responses.Examples.PresetQueryResponseExamples), StatusCodes.Status200OK)]
		// [SwaggerResponseExample(typeof(v1.Responses.ErrorResponse), typeof(v1.Responses.Examples.BadRequestErrorResponseExamples), StatusCodes.Status400BadRequest)]
		// V3 WARNING: Potentially breaking change
		//[ProducesResponseType(typeof(v1.Responses.ErrorResponse), StatusCodes.Status404NotFound)]
		//[SwaggerResponseExample(typeof(v1.Responses.ErrorResponse), typeof(v1.Responses.Examples.PresetNotFoundErrorResponseExample), StatusCodes.Status404NotFound)]
		// [SwaggerResponseExample(typeof(v1.Responses.ErrorResponse), typeof(v1.Responses.Examples.ServerErrorResponseExample), StatusCodes.Status500InternalServerError)]

	}
	
	#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	internal async Task<IResult> HandleAsync(
		[FromRoute] string? preset,
		[FromBody] PresetQueryRequest? request,
		IValidator<PresetQueryRequest> validator,
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
					ErrorResponse.Create(Categories.RequestError)
						.AddParameterError(nameof(preset), Messages.IsRequired)
				);
			}
			
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

			if (!presetOptions.Value.Presets.TryGetValue(preset, out var presetOption))
			{
				return Results.NotFound(null);
				// V3 WARNING: Potentially breaking change
				// Required due to UI Module registering Antiforgery
				//return this.NotFound(
				//	v1.Responses.ErrorResponse.Create(Constants.Errors.Categories.PresetDoesntExist)
				//);
			}

			var operationResults = await binsService.FitBinsAsync(
				presetOption.Bins, 
				request.Items!,
				new LegacyFittingParameters
				{
					FindSmallestBinOnly = true,
					ReportFittedItems = false,
					ReportUnfittedItems = false
				}
			);

			return Results.Ok(
				QueryResponse.Create(presetOption.Bins, request.Items!, operationResults)
			);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An exception occurred in {endpoint} endpoint", "Query by Preset");
			return Results.InternalServerError(
				ErrorResponse.Create(Categories.ServerError)
					.AddExceptionError(ex)
			);
		}
	}
}
