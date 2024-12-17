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

namespace Binacle.Net.Api.v2.Endpoints.Pack;

/// <summary>
/// Pack by Preset endpoint
/// </summary>
[ApiVersion(v2.ApiVersion.Number, Deprecated = v2.ApiVersion.IsDeprecated)]
[Route("api/v{version:apiVersion}/[namespace]")]
public class ByPreset : EndpointWithRequest<v2.Requests.PresetPackRequestWithBody>
{
	private readonly IOptions<BinPresetOptions> presetOptions;
	private readonly IValidator<v2.Requests.PresetPackRequest> validator;
	private readonly IBinsService binsService;
	private readonly ILogger<ByPreset> logger;

	/// <summary>
	/// Pack by Preset endpoint
	/// </summary>
	/// <param name="presetOptions"></param>
	/// <param name="validator"></param>
	/// <param name="binsService"></param>
	/// <param name="logger"></param>
	public ByPreset(
		IOptions<BinPresetOptions> presetOptions,
		IValidator<v2.Requests.PresetPackRequest> validator,
		IBinsService binsService,
		ILogger<ByPreset> logger
	  )
	{
		this.validator = validator;
		this.binsService = binsService;
		this.logger = logger;
		this.presetOptions = presetOptions;
	}

	/// <summary>
	/// Pack items using a specified bin preset.
	/// </summary>
	/// <returns>An array of results indicating the result per bin</returns>
	/// <remarks>
	/// Example request using the "rectangular-cuboids" preset:
	///     
	///     POST /api/v2/pack/by-preset/rectangular-cuboids
	///		{
	///			"parameters": {
	///				"neverReportUnpackedItems": false,
	///				"optInToEarlyFails": false,
	///				"stopAtSmallestBin": false,
	///				"reportPackedItemsOnlyWhenFullyPacked": false
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
	///				},
	///				{
	///					"id": "box_3",
	///					"quantity": 1,
	///					"length": 12,
	///					"width": 10,
	///					"height": 15
	///				}
	///			]
	///		}
	/// 
	/// </remarks>
	/// <response code="200"> <b>OK</b>
	/// <br />
	/// <p>
	///		An array of results indicating the result per bin.
	/// </p>
	/// </response>
	/// <response code="400"> <b>Bad Request</b>
	/// <br/> 
	/// <p>
	///		If the request is invalid.
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
	[HttpPost("by-preset/{preset}")]
	[Consumes("application/json")]
	[Produces("application/json")]
	[MapToApiVersion(v2.ApiVersion.Number)]
	[SwaggerRequestExample(typeof(v2.Requests.PresetPackRequest), typeof(v2.Requests.Examples.PresetPackRequestExample))]

	[ProducesResponseType(typeof(v2.Responses.PackResponse), StatusCodes.Status200OK)]
	[SwaggerResponseExample(typeof(v2.Responses.PackResponse), typeof(v2.Responses.Examples.PresetPackResponseExamples), StatusCodes.Status200OK)]

	[ProducesResponseType(typeof(v2.Responses.ErrorResponse), StatusCodes.Status400BadRequest)]
	[SwaggerResponseExample(typeof(v2.Responses.ErrorResponse), typeof(v2.Responses.Examples.BadRequestErrorResponseExamples), StatusCodes.Status400BadRequest)]

	[ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]

	[ProducesResponseType(typeof(v2.Responses.ErrorResponse), StatusCodes.Status500InternalServerError)]
	[SwaggerResponseExample(typeof(v2.Responses.ErrorResponse), typeof(v2.Responses.Examples.ServerErrorResponseExample), StatusCodes.Status500InternalServerError)]
	public override async Task<IActionResult> HandleAsync(v2.Requests.PresetPackRequestWithBody request, CancellationToken cancellationToken = default)
	{
		try
		{
			if (request is null || request.Body is null)
			{
				return this.BadRequest(
					Models.Response.ParameterError(nameof(request), Constants.Errors.Messages.MalformedRequestBody, Constants.Errors.Categories.RequestError)
					);
			}

			await this.validator.ValidateAndAddToModelStateAsync(request.Body, this.ModelState, cancellationToken);

			if (!this.ModelState.IsValid)
			{
				return this.BadRequest(
					Models.Response.ValidationError(this.ModelState, Constants.Errors.Categories.ValidationError)
					);
			}

			if (!this.presetOptions.Value.Presets.TryGetValue(request.Preset!, out var presetOption))
			{
				return this.NotFound(null);
			}

			var operationResults = this.binsService.PackBins(
				presetOption.Bins,
				request.Body.Items!,
				new Api.Models.PackingParameters
				{
					Algorithm = Algorithm.FFD,
					StopAtSmallestBin = request.Body.Parameters?.StopAtSmallestBin ?? false,
					NeverReportUnpackedItems = request.Body.Parameters?.NeverReportUnpackedItems ?? false,
					OptInToEarlyFails = request.Body.Parameters?.OptInToEarlyFails ?? false,
					ReportPackedItemsOnlyWhenFullyPacked = request.Body.Parameters?.ReportPackedItemsOnlyWhenFullyPacked ?? false,
				}
			);

			return this.Ok(
				v2.Responses.PackResponse.Create(
					presetOption.Bins,
					request.Body.Items!,
					request.Body.Parameters,
					operationResults
				)
			);

		}
		catch (Exception ex)
		{
			this.logger.LogError(ex, "An exception occurred in {endpoint} endpoint", "Pack by Preset");
			return this.InternalServerError(
				Models.Response.ExceptionError(ex, Constants.Errors.Categories.ServerError)
				);
		}
	}
}

