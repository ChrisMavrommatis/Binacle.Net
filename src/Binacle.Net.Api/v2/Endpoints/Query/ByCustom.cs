using Asp.Versioning;
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

namespace Binacle.Net.Api.v2.Endpoints.Query;


/// <summary>
/// Query by Custom endpoint
/// </summary>
[ApiVersion(v2.ApiVersion.Number)]
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
	[MapToApiVersion(v2.ApiVersion.Number)]
	[SwaggerRequestExample(typeof(CustomQueryRequest), typeof(CustomQueryRequestExample))]

	[ProducesResponseType(typeof(Response<Bin?>), StatusCodes.Status200OK)]
	[SwaggerResponseExample(typeof(Response<Bin?>), typeof(CustomQueryResponseExamples), StatusCodes.Status200OK)]

	[ProducesResponseType(typeof(Response<List<IApiError>>), StatusCodes.Status400BadRequest)]
	[SwaggerResponseExample(typeof(Response<List<IApiError>>), typeof(BadRequestErrorResponseExamples), StatusCodes.Status400BadRequest)]

	[ProducesResponseType(typeof(Response<List<IApiError>>), StatusCodes.Status500InternalServerError)]
	[SwaggerResponseExample(typeof(Response<List<IApiError>>), typeof(ServerErrorResponseExample), StatusCodes.Status500InternalServerError)]
	public override async Task<IActionResult> HandleAsync(CustomQueryRequestWithBody request, CancellationToken cancellationToken = default)
	{
		try
		{
			if (request is null || request.Body is null)
			{
				return this.BadRequest(
					Responses.Response.ParameterError(nameof(request), Constants.Errors.Messages.MalformedRequestBody, Constants.Errors.Categories.RequestError)
					);
			}

			await this.validator.ValidateAndAddToModelStateAsync(request.Body, this.ModelState, cancellationToken);

			if (!this.ModelState.IsValid)
			{
				return this.BadRequest(
					Responses.Response.ValidationError(this.ModelState, Constants.Errors.Categories.ValidationError)
					);
			}

			var operationResult = this.lockerService.FindFittingBin(request.Body.Bins, request.Body.Items);

			return this.Ok(
				Responses.Response.FromResult(operationResult)
				);
		}
		catch (Exception ex)
		{
			this.logger.LogError(ex, "An exception occurred in {endpoint} endpoint", "Query by Custom");
			return this.InternalServerError(
				Responses.Response.ExceptionError(ex, Constants.Errors.Categories.ServerError)
				);
		}
	}
}
