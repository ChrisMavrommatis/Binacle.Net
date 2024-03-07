using Asp.Versioning;
using Binacle.Net.Api.ExtensionMethods;
using Binacle.Net.Api.Models.Requests;
using Binacle.Net.Api.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.Api.Controllers;

public partial class QueryController
{
	/// <summary>
	/// Perform a bin fit query using custom bins.
	/// </summary>
	/// <param name="request"></param>
	/// <returns>The bin that fits all of the items</returns>
	/// <remarks>
	/// Example request:
	///     
	///     POST /api/v1/query/
	///     {
	///			"bins" : [
	///			  {
	///			    "id" : "bin_1",
	///			    "length": 35,
	///             "width": 30,
	///             "height": 25
	///			  },
	///			  {
	///			    "id" : "bin_2",
	///			    "length": 15,
	///             "width": 25,
	///             "height": 25
	///			  }
	///			],
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
	/// <response code="500">If an unexpected error occurs</response>
	[HttpPost]
	[Consumes("application/json")]
	[Produces("application/json")]
	[MapToApiVersion("1.0")]
	[ProducesResponseType(typeof(QueryResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> Index(QueryRequest request)
	{
		try
		{
			//if (request is null)
			//{
			//	return this.BadRequest(
			//		ErrorResponse.Create("Malformed request")
			//		.AddParameterError(nameof(request), Constants.ErrorMessages.MalformedRequestBody)
			//		);
			//}

			await this.queryRequestValidator.ValidateAndAddToModelStateAsync(request, this.ModelState);

			if (!this.ModelState.IsValid)
			{
				return this.BadRequest(
					ErrorResponse.Create("One or More Validation errors occurred.")
					.AddModelStateErrors(this.ModelState)
					);
			}

			var operationResult = this.lockerService.FindFittingBin(request.Bins, request.Items);

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
