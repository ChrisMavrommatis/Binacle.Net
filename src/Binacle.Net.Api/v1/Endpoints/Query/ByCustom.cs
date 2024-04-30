using Asp.Versioning;
using Binacle.Net.Api.Services;
using Binacle.Net.Api.v1.Requests;
using Binacle.Net.Api.v1.Requests.Examples;
using Binacle.Net.Api.v1.Responses;
using Binacle.Net.Api.v1.Responses.Examples;
using ChrisMavrommatis.Endpoints;
using ChrisMavrommatis.FluentValidation;
using ChrisMavrommatis.SwaggerExamples.Attributes;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.Api.v1.Endpoints.Query;

/// <summary>
/// Query by Custom endpoint
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[namespace]")]
public class ByCustom : EndpointWithRequest<CustomQueryRequestWithBody>
{
	private readonly IValidator<CustomQueryRequest> validator;
	private readonly ILockerService lockerService;
	private readonly ILogger<ByCustom> logger;

	/// <summary>
	/// Query by Custom endpoint
	/// </summary>
	/// <param name="validator"></param>
	/// <param name="lockerService"></param>
	/// <param name="logger"></param>
	public ByCustom(
		IValidator<CustomQueryRequest> validator,
		ILockerService lockerService,
		ILogger<ByCustom> logger
	  )
	{
		this.validator = validator;
		this.lockerService = lockerService;
		this.logger = logger;
	}

	/// <summary>
	/// Perform a bin fit query using custom bins.
	/// </summary>
	/// <returns>The bin that fits all of the items, or empty</returns>
	/// <remarks>
	/// Example request:
	///     
	///     POST /api/v1/query/
	///     {
	///			"bins" : [
	///			  {
	///			     "id": "custom_bin_1",
	///			     "length": 10,
	///              "width": 40,
	///              "height": 60
	///			   },
	///			   {
	///			     "id": "custom_bin_2",
	///			     "length": 20,
	///              "width": 40,
	///              "height": 60
	///			   }
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
	/// <response code="200"> <b>OK</b>
	/// <br />
	/// <p>
	///		Returns the bin that fits all of the items, or empty if they don't fit.
	/// </p>
	/// </response>
	/// <response code="400"> <b>Bad Request</b>
	/// <br/> 
	/// If the request is invalid.
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
	[HttpPost("by-custom")]
	[Consumes("application/json")]
	[Produces("application/json")]
	[MapToApiVersion("1.0")]
	[SwaggerRequestExample(typeof(CustomQueryRequest), typeof(CustomQueryRequestExample))]

	[ProducesResponseType(typeof(QueryResponse), StatusCodes.Status200OK)]
	[SwaggerResponseExample(typeof(QueryResponse), typeof(CustomQueryResponseExamples), StatusCodes.Status200OK)]

	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
	[SwaggerResponseExample(typeof(ErrorResponse), typeof(BadRequestErrorResponseExamples), StatusCodes.Status400BadRequest)]

	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
	[SwaggerResponseExample(typeof(ErrorResponse), typeof(ServerErrorResponseExample), StatusCodes.Status500InternalServerError)]
	public override async Task<IActionResult> HandleAsync(CustomQueryRequestWithBody request, CancellationToken cancellationToken = default)
	{
		try
		{
			if (request is null || request.Body is null)
			{
				return this.BadRequest(
					ErrorResponse.Create(Constants.Errors.Categories.RequestError)
					.AddParameterError(nameof(request), Constants.Errors.Messages.MalformedRequestBody)
					);
			}

			await this.validator.ValidateAndAddToModelStateAsync(request.Body, this.ModelState, cancellationToken);

			if (!this.ModelState.IsValid)
			{
				return this.BadRequest(
					ErrorResponse.Create(Constants.Errors.Categories.ValidationError)
					.AddModelStateErrors(this.ModelState)
					);
			}

			var operationResult = this.lockerService.FindFittingBin(request.Body.Bins, request.Body.Items);

			return this.Ok(
				QueryResponse.Create(operationResult)
				);
		}
		catch (Exception ex)
		{
			this.logger.LogError(ex, "An exception occurred in {endpoint} endpoint", "Query by Custom");
			return this.InternalServerError(
				ErrorResponse.Create(Constants.Errors.Categories.ServerError)
				.AddExceptionError(ex)
				);
		}
	}
}
