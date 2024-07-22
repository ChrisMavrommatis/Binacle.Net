using Asp.Versioning;
using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.Api.Models;
using Binacle.Net.Api.Services;
using Binacle.Net.Api.v2.Models.Errors;
using Binacle.Net.Api.v2.Requests;
using Binacle.Net.Api.v2.Requests.Examples;
using Binacle.Net.Api.v2.Responses;
using Binacle.Net.Api.v2.Responses.Examples;
using ChrisMavrommatis.Endpoints;
using ChrisMavrommatis.FluentValidation;
using ChrisMavrommatis.SwaggerExamples.Attributes;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Binacle.Net.Api.v2.Endpoints.Query;

/// <summary>
/// Query by Preset endpoint
/// </summary>
[ApiVersion(v2.ApiVersion.Number)]
[Route("api/v{version:apiVersion}/[namespace]")]
public class ByPreset : EndpointWithRequest<PresetQueryRequestWithBody>
{
	private readonly IOptions<BinPresetOptions> presetOptions;
	private readonly IValidator<PresetQueryRequest> validator;
	private readonly ILockerService lockerService;
	private readonly ILogger<ByPreset> logger;

	/// <summary>
	/// Query by Preset endpoint
	/// </summary>
	/// <param name="presetOptions"></param>
	/// <param name="validator"></param>
	/// <param name="lockerService"></param>
	/// <param name="logger"></param>
	public ByPreset(
		IOptions<BinPresetOptions> presetOptions,
		IValidator<PresetQueryRequest> validator,
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
	/// Perform a bin fit query using a specified bin preset.
	/// </summary>
	/// <remarks>
	/// Example request using the "rectangular-cuboids" preset:
	///     
	///     POST /api/v1/query/presets/rectangular-cuboids
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
	///		Returns the bin that fits all of the items, or empty if they don't fit.
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

	[SwaggerRequestExample(typeof(PresetQueryRequest), typeof(PresetQueryRequestExample))]

	[ProducesResponseType(typeof(Response<Bin?>), StatusCodes.Status200OK)]
	[SwaggerResponseExample(typeof(Response<Bin?>), typeof(PresetQueryResponseExamples), StatusCodes.Status200OK)]

	[ProducesResponseType(typeof(Response<List<IApiError>>), StatusCodes.Status400BadRequest)]
	[SwaggerResponseExample(typeof(Response<List<IApiError>>), typeof(BadRequestErrorResponseExamples), StatusCodes.Status400BadRequest)]

	[ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]

	[ProducesResponseType(typeof(Response<List<IApiError>>), StatusCodes.Status500InternalServerError)]
	[SwaggerResponseExample(typeof(Response<List<IApiError>>), typeof(ServerErrorResponseExample), StatusCodes.Status500InternalServerError)]
	public override async Task<IActionResult> HandleAsync(PresetQueryRequestWithBody request, CancellationToken cancellationToken = default)
	{
		try
		{
			if (request is null || request.Body is null)
			{
				return this.BadRequest(
					Responses.Response.ParameterError(nameof(request), Constants.Errors.Messages.MalformedRequestBody, Constants.Errors.Categories.RequestError)
					);
			}

			if (string.IsNullOrWhiteSpace(request.Preset))
			{
				return this.BadRequest(
					Responses.Response.ParameterError(nameof(request.Preset), Constants.Errors.Messages.IsRequired, Constants.Errors.Categories.RequestError)
					);
			}

			await this.validator.ValidateAndAddToModelStateAsync(request.Body, this.ModelState, cancellationToken);

			if (!this.ModelState.IsValid)
			{
				return this.BadRequest(
					Responses.Response.ValidationError(this.ModelState, Constants.Errors.Categories.ValidationError)
					);
			}

			if (!this.presetOptions.Value.Presets.TryGetValue(request.Preset, out var presetOption))
			{
				return this.NotFound(null);
			}

			var operationResult = this.lockerService.FindFittingBin(presetOption.Bins, request.Body.Items);
			return this.Ok(
				Responses.Response.FromResult(operationResult)
			);
		}
		catch (Exception ex)
		{
			this.logger.LogError(ex, "An exception occurred in {endpoint} endpoint", "Query by Preset");
			return this.InternalServerError(
				Responses.Response.ExceptionError(ex, Constants.Errors.Categories.ServerError)
				);
		}
	}
}
