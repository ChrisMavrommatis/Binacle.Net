using Asp.Versioning;
using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.Api.Models.Requests;
using Binacle.Net.Api.Models.Responses;
using Binacle.Net.Api.Services;
using ChrisMavrommatis.Endpoints;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Binacle.Net.Api.Endpoints.Query;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[namespace]")]
public class ByPreset : EndpointWithRequest<PresetQueryRequestWithBody>
{
	private readonly IOptions<BinPresetOptions> presetOptions;
	private readonly IValidator<PresetQueryRequest> presetQueryRequestValidator;
	private readonly ILockerService lockerService;

	public ByPreset(
		IOptions<BinPresetOptions> presetOptions,
		IValidator<PresetQueryRequest> presetQueryRequestValidator,
		ILockerService lockerService
	  )
	{
		this.presetQueryRequestValidator = presetQueryRequestValidator;
		this.lockerService = lockerService;
		this.presetOptions = presetOptions;
	}

	/// <summary>
	/// Perform a bin fit query using a specified bin preset.
	/// </summary>
	/// <returns>The bin that fits all of the items</returns>
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
	///           }
	///         ]
	///     }
	/// 
	/// </remarks>
	/// <response code="200">Returns the bin that fits all of the items</response>
	/// <response code="400">If the request is invalid</response>
	/// <response code="404">If the preset does not exist</response>
	/// <response code="500">If an unexpected error occurs</response>
	[HttpPost]
	[Route("by-preset/{preset}")]
	[Consumes("application/json")]
	[Produces("application/json")]
	[MapToApiVersion("1.0")]
	[ProducesResponseType(typeof(QueryResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
	public override async Task<IActionResult> HandleAsync(PresetQueryRequestWithBody request, CancellationToken cancellationToken = default)
	{
		try
		{
			if (request is null || request.Body is null)
			{
				return this.BadRequest(
					ErrorResponse.Create("Malformed request")
					.AddParameterError(nameof(request), Constants.ErrorMessages.MalformedRequestBody)
					);
			}

			if (string.IsNullOrWhiteSpace(request.Preset))
			{
				return this.BadRequest(
					ErrorResponse.Create("Malformed request")
					.AddParameterError(nameof(request.Preset), Constants.ErrorMessages.IsRequired)
					);
			}

		
			await this.presetQueryRequestValidator.ValidateAndAddToModelStateAsync(request.Body, this.ModelState, cancellationToken);

			if (!this.ModelState.IsValid)
			{
				return this.BadRequest(
				   ErrorResponse.Create("One or More Validation errors occurred.")
					.AddModelStateErrors(this.ModelState)
					);
			}

			if (!this.presetOptions.Value.Presets.TryGetValue(request.Preset, out var presetOption))
			{
				return this.NotFound(
					ErrorResponse.Create("Preset not found.")
					.AddParameterError(nameof(request.Preset), string.Format("preset '{0}' does not exist.", request.Preset))
					);
			}

			var operationResult = this.lockerService.FindFittingBin(presetOption.Bins, request.Body.Items);
			return this.Ok(
				QueryResponse.Create(operationResult)
			);
		}
		catch (Exception ex)
		{
			return this.InternalServerError(
				ErrorResponse.Create("An internal server error occurred while processing the request.")
				.AddExceptionError(ex)
				);
		}
	}
}
