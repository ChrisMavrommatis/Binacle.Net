using Asp.Versioning;
using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.Api.Services;
using ChrisMavrommatis.Endpoints;
using ChrisMavrommatis.FluentValidation;
using ChrisMavrommatis.SwaggerExamples.Attributes;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Binacle.Net.Api.v1.Endpoints.Query;

/// <summary>
/// Query by Preset endpoint
/// </summary>
[ApiVersion(v1.ApiVersion.Number, Deprecated = v1.ApiVersion.IsDeprecated)]
[Route("api/v{version:apiVersion}/[namespace]")]
public class ByPreset : EndpointWithRequest<v1.Requests.PresetQueryRequestWithBody>
{
	private readonly IOptions<BinPresetOptions> presetOptions;
	private readonly IValidator<v1.Requests.PresetQueryRequest> validator;
	private readonly IBinsService binsService;
	private readonly ILogger<ByPreset> logger;

	/// <summary>
	/// Query by Preset endpoint
	/// </summary>
	/// <param name="presetOptions"></param>
	/// <param name="validator"></param>
	/// <param name="binsService"></param>
	/// <param name="logger"></param>
	public ByPreset(
		IOptions<BinPresetOptions> presetOptions,
		IValidator<v1.Requests.PresetQueryRequest> validator,
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
	/// Perform a bin fit query using a specified bin preset.
	/// </summary>
	/// <remarks>
	/// Example request using the "rectangular-cuboids" preset:
	///     
	///     POST /api/v1/query/by-preset/rectangular-cuboids
	///     {
	///         "items": [
	///           {
	///             "id": "box_1",
	///             "quantity": 2,
	///             "length": 2,
	///             "width": 5,
	///             "height": 10
	///           },
	///           {
	///             "id": "box_2",
	///             "quantity": 1,
	///             "length": 12,
	///             "width": 15,
	///             "height": 10
	///           },
	///           {
	///             "id": "box_3",
	///             "quantity": 1,
	///             "length": 12,
	///             "width": 10,
	///             "height": 15
	///           }
	///         ]
	///     }
	/// 
	/// </remarks>
	/// <response code="200"> <b>OK</b>
	/// <br />
	/// <p>
	///		Returns the bin that fits all the items, or empty if they don't fit.
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
	[MapToApiVersion(v1.ApiVersion.Number)]

	[SwaggerRequestExample(typeof(v1.Requests.PresetQueryRequest), typeof(v1.Requests.Examples.PresetQueryRequestExample))]

	[ProducesResponseType(typeof(v1.Responses.QueryResponse), StatusCodes.Status200OK)]
	[SwaggerResponseExample(typeof(v1.Responses.QueryResponse), typeof(v1.Responses.Examples.PresetQueryResponseExamples), StatusCodes.Status200OK)]

	[ProducesResponseType(typeof(v1.Responses.ErrorResponse), StatusCodes.Status400BadRequest)]
	[SwaggerResponseExample(typeof(v1.Responses.ErrorResponse), typeof(v1.Responses.Examples.BadRequestErrorResponseExamples), StatusCodes.Status400BadRequest)]

	// V3 WARNING: Potentially breaking change
	[ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
	//[ProducesResponseType(typeof(v1.Responses.ErrorResponse), StatusCodes.Status404NotFound)]
	//[SwaggerResponseExample(typeof(v1.Responses.ErrorResponse), typeof(v1.Responses.Examples.PresetNotFoundErrorResponseExample), StatusCodes.Status404NotFound)]

	[ProducesResponseType(typeof(v1.Responses.ErrorResponse), StatusCodes.Status500InternalServerError)]
	[SwaggerResponseExample(typeof(v1.Responses.ErrorResponse), typeof(v1.Responses.Examples.ServerErrorResponseExample), StatusCodes.Status500InternalServerError)]
	public override async Task<IActionResult> HandleAsync(v1.Requests.PresetQueryRequestWithBody request, CancellationToken cancellationToken = default)
	{
		try
		{
			if (request is null || request.Body is null)
			{
				return this.BadRequest(
					v1.Responses.ErrorResponse.Create(Constants.Errors.Categories.RequestError)
					.AddParameterError(nameof(request), Constants.Errors.Messages.MalformedRequestBody)
				);
			}

			if (string.IsNullOrWhiteSpace(request.Preset))
			{
				return this.BadRequest(
					v1.Responses.ErrorResponse.Create(Constants.Errors.Categories.RequestError)
					.AddParameterError(nameof(request.Preset), Constants.Errors.Messages.IsRequired)
				);
			}


			await this.validator.ValidateAndAddToModelStateAsync(request.Body, this.ModelState, cancellationToken);

			if (!this.ModelState.IsValid)
			{
				return this.BadRequest(
				   v1.Responses.ErrorResponse.Create(Constants.Errors.Categories.ValidationError)
					.AddModelStateErrors(this.ModelState)
				);
			}

			if (!this.presetOptions.Value.Presets.TryGetValue(request.Preset, out var presetOption))
			{
				return this.NotFound(null);
				// V3 WARNING: Potentially breaking change
				// Required due to UI Module registering Antiforgery
				//return this.NotFound(
				//	v1.Responses.ErrorResponse.Create(Constants.Errors.Categories.PresetDoesntExist)
				//);
			}

			var operationResults = this.binsService.FitBins(
				presetOption.Bins, 
				request.Body.Items!,
				new Api.Models.FittingParameters
				{
					FindSmallestBinOnly = true,
					ReportFittedItems = false,
					ReportUnfittedItems = false
				}
			);

			return this.Ok(
				v1.Responses.QueryResponse.Create(presetOption.Bins, request.Body.Items!, operationResults)
			);
		}
		catch (Exception ex)
		{
			this.logger.LogError(ex, "An exception occurred in {endpoint} endpoint", "Query by Preset");
			return this.InternalServerError(
				v1.Responses.ErrorResponse.Create(Constants.Errors.Categories.ServerError)
				.AddExceptionError(ex)
				);
		}
	}
}
