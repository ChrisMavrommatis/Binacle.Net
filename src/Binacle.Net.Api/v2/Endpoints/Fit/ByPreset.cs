using Asp.Versioning;
using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.Api.Models;
using Binacle.Net.Api.Services;
using ChrisMavrommatis.Endpoints;
using ChrisMavrommatis.FluentValidation;
using ChrisMavrommatis.SwaggerExamples.Attributes;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Binacle.Net.Api.v2.Endpoints.Fit;

/// <summary>
/// Fit by Preset endpoint
/// </summary>
[ApiVersion(v2.ApiVersion.Number, Deprecated = v2.ApiVersion.IsDeprecated)]
[Route("api/v{version:apiVersion}/[namespace]")]
public class ByPreset : EndpointWithRequest<v2.Requests.PresetFitRequestWithBody>
{
	private readonly IOptions<BinPresetOptions> presetOptions;
	private readonly IValidator<v2.Requests.PresetFitRequest> validator;
	private readonly ILockerService lockerService;
	private readonly ILogger<ByPreset> logger;

	/// <summary>
	/// Fit by Preset endpoint
	/// </summary>
	/// <param name="presetOptions"></param>
	/// <param name="validator"></param>
	/// <param name="lockerService"></param>
	/// <param name="logger"></param>
	public ByPreset(
		IOptions<BinPresetOptions> presetOptions,
		IValidator<v2.Requests.PresetFitRequest> validator,
		ILockerService lockerService,
		ILogger<ByPreset> logger
	  )
	{
		this.validator = validator;
		this.lockerService = lockerService;
		this.logger = logger;
		this.presetOptions = presetOptions;
	}

	/// <summary>
	/// Perform a bin fit function using a specified bin preset.
	/// </summary>
	/// <remarks>
	/// Example request using the "rectangular-cuboids" preset:
	///     
	///     POST /api/v2/fit/by-preset/rectangular-cuboids
	///		{
	///			"parameters": {
	///				"reportFittedItems": true,	
	///				"reportUnfittedItems": true,
	///				"findSmallestBinOnly": false
	///			},
	///			"items": [
	///				{
	///					"id": "box_1",
	///					"quantity": 2,
	///					"length": 2,
	///					"width": 5,
	///					"height": 10
	///				},
	///				{
	///					"id": "box_2",
	///					"quantity": 1,
	///					"length": 12,
	///					"width": 15,
	///					"height": 10
	///				}
	///			]
	///		}
	/// 
	/// </remarks>
	/// <response code="200"> <b>OK</b>
	/// <br />
	/// <p>
	///		An array of results indicating if a bin can accommodate all of the items.
	/// </p>
	/// </response>
	/// <response code="400"> <b>Bad Request</b>
	/// <br/> 
	/// <p>
	///		If the request is invalid.
	/// </p>
	/// </response>
	/// <response code="404"> <b>Not Found</b>
	/// <br />
	/// <p>
	///		If the preset does not exist.
	/// </p>
	/// </response>
	/// <response code="500"> <b>Internal Server Error</b>
	/// <br />
	/// <p>
	///		If an unexpected error occurs.
	/// </p>
	/// <p>
	///		Exception details will only be shown when in a development environment.
	/// </p>
	/// </response>
	[HttpPost]
	[Route("by-preset/{preset}")]
	[Consumes("application/json")]
	[Produces("application/json")]
	[MapToApiVersion(v2.ApiVersion.Number)]

	[SwaggerRequestExample(typeof(v2.Requests.PresetFitRequest), typeof(v2.Requests.Examples.PresetFitRequestExample))]

	[ProducesResponseType(typeof(v2.Responses.FitResponse), StatusCodes.Status200OK)]
	[SwaggerResponseExample(typeof(v2.Responses.FitResponse), typeof(v2.Responses.Examples.PresetFitResponseExamples), StatusCodes.Status200OK)]

	[ProducesResponseType(typeof(v2.Responses.ErrorResponse), StatusCodes.Status400BadRequest)]
	[SwaggerResponseExample(typeof(v2.Responses.ErrorResponse), typeof(v2.Responses.Examples.BadRequestErrorResponseExamples), StatusCodes.Status400BadRequest)]

	[ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]

	[ProducesResponseType(typeof(v2.Responses.ErrorResponse), StatusCodes.Status500InternalServerError)]
	[SwaggerResponseExample(typeof(v2.Responses.ErrorResponse), typeof(v2.Responses.Examples.ServerErrorResponseExample), StatusCodes.Status500InternalServerError)]
	public override async Task<IActionResult> HandleAsync(v2.Requests.PresetFitRequestWithBody request, CancellationToken cancellationToken = default)
	{
		try
		{
			if (request is null || request.Body is null)
			{
				return this.BadRequest(
					Models.Response.ParameterError(nameof(request), Constants.Errors.Messages.MalformedRequestBody, Constants.Errors.Categories.RequestError)
					);
			}

			if (string.IsNullOrWhiteSpace(request.Preset))
			{
				return this.BadRequest(
					Models.Response.ParameterError(nameof(request.Preset), Constants.Errors.Messages.IsRequired, Constants.Errors.Categories.RequestError)
					);
			}

			await this.validator.ValidateAndAddToModelStateAsync(request.Body, this.ModelState, cancellationToken);

			if (!this.ModelState.IsValid)
			{
				return this.BadRequest(
					Models.Response.ValidationError(this.ModelState, Constants.Errors.Categories.ValidationError)
					);
			}

			if (!this.presetOptions.Value.Presets.TryGetValue(request.Preset, out var presetOption))
			{
				return this.NotFound(null);
			}

			var operationResults = this.lockerService.FitBins(
				presetOption.Bins,
				request.Body.Items!,
				new FittingParameters
				{
					FindSmallestBinOnly = request.Body.Parameters?.FindSmallestBinOnly ?? true,
					ReportFittedItems = request.Body.Parameters?.ReportFittedItems ?? false,
					ReportUnfittedItems = request.Body.Parameters?.FindSmallestBinOnly ?? false,
				}
			);
			return this.Ok( 
				v2.Responses.FitResponse.Create(
					presetOption.Bins,
					request.Body.Items!,
					request.Body.Parameters,
					operationResults
				)
			);

		}
		catch (Exception ex)
		{
			this.logger.LogError(ex, "An exception occurred in {endpoint} endpoint", "Query by Preset");
			return this.InternalServerError(
				Models.Response.ExceptionError(ex, Constants.Errors.Categories.ServerError)
				);
		}
	}
}
