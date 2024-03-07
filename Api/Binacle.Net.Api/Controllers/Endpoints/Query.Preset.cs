using Asp.Versioning;
using Binacle.Net.Api.ExtensionMethods;
using Binacle.Net.Api.Models.Requests;
using Binacle.Net.Api.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.Api.Controllers;

public partial class QueryController
{
	/// <summary>
	/// Perform a bin fit query using a specified bin preset.
	/// </summary>
	/// <param name="preset"></param>
	/// <param name="request"></param>
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
	[Route("presets/{preset}")]
	[Consumes("application/json")]
	[Produces("application/json")]
	[MapToApiVersion("1.0")]
	[ProducesResponseType(typeof(QueryResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> Preset(
		string preset,
		[FromBody] PresetQueryRequest request
		)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(preset))
			{
				return this.BadRequest(
					ErrorResponse.Create("Malformed request")
					.AddParameterError(nameof(preset), Constants.ErrorMessages.IsRequired)
					);
			}

			//if (request is null)
			//{
			//	return this.BadRequest(
			//		ErrorResponse.Create("Malformed request")
			//		.AddParameterError(nameof(request), Constants.ErrorMessages.MalformedRequestBody)
			//		);
			//}
			await this.presetQueryRequestValidator.ValidateAndAddToModelStateAsync(request, this.ModelState);

			if (!this.ModelState.IsValid)
			{
				return this.BadRequest(
				   ErrorResponse.Create("One or More Validation errors occurred.")
					.AddModelStateErrors(this.ModelState)
					);
			}

			if (!this.presetOptions.Value.Presets.TryGetValue(preset, out var presetOption))
			{
				return this.NotFound(
					ErrorResponse.Create("Preset not found.")
					.AddParameterError(nameof(preset), string.Format("preset '{0}' does not exist.", preset))
					);
			}

			var operationResult = this.lockerService.FindFittingBin(presetOption.Bins, request.Items);
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
