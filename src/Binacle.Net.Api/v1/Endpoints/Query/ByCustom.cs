using Asp.Versioning;
using Binacle.Net.Api.Services;
using ChrisMavrommatis.Endpoints;
using ChrisMavrommatis.FluentValidation;
using ChrisMavrommatis.SwaggerExamples.Attributes;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.Api.v1.Endpoints.Query;

/// <summary>
/// Query by Custom endpoint
/// </summary>
[ApiVersion(v1.ApiVersion.Number, Deprecated = v1.ApiVersion.IsDeprecated)]
[Route("api/v{version:apiVersion}/[namespace]")]
public class ByCustom : EndpointWithRequest<v1.Requests.CustomQueryRequestWithBody>
{
	private readonly IValidator<v1.Requests.CustomQueryRequest> validator;
	private readonly IBinsService binsService;
	private readonly ILogger<ByCustom> logger;

	/// <summary>
	/// Query by Custom endpoint
	/// </summary>
	/// <param name="validator"></param>
	/// <param name="binsService"></param>
	/// <param name="logger"></param>
	public ByCustom(
		IValidator<v1.Requests.CustomQueryRequest> validator,
		IBinsService binsService,
		ILogger<ByCustom> logger
	  )
	{
		this.validator = validator;
		this.binsService = binsService;
		this.logger = logger;
	}

	/// <summary>
	/// Perform a bin fit query using custom bins.
	/// </summary>
	/// <returns>The bin that fits all the items, or empty</returns>
	/// <remarks>
	/// Example request:
	///     
	///     POST /api/v1/query/by-custom
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
	[HttpPost("by-custom")]
	[Consumes("application/json")]
	[Produces("application/json")]
	[MapToApiVersion(v1.ApiVersion.Number)]
	[SwaggerRequestExample(typeof(v1.Requests.CustomQueryRequest), typeof(v1.Requests.Examples.CustomQueryRequestExample))]

	[ProducesResponseType(typeof(v1.Responses.QueryResponse), StatusCodes.Status200OK)]
	[SwaggerResponseExample(typeof(v1.Responses.QueryResponse), typeof(v1.Responses.Examples.CustomQueryResponseExamples), StatusCodes.Status200OK)]

	[ProducesResponseType(typeof(v1.Responses.ErrorResponse), StatusCodes.Status400BadRequest)]
	[SwaggerResponseExample(typeof(v1.Responses.ErrorResponse), typeof(v1.Responses.Examples.BadRequestErrorResponseExamples), StatusCodes.Status400BadRequest)]

	[ProducesResponseType(typeof(v1.Responses.ErrorResponse), StatusCodes.Status500InternalServerError)]
	[SwaggerResponseExample(typeof(v1.Responses.ErrorResponse), typeof(v1.Responses.Examples.ServerErrorResponseExample), StatusCodes.Status500InternalServerError)]
	public override async Task<IActionResult> HandleAsync(v1.Requests.CustomQueryRequestWithBody request, CancellationToken cancellationToken = default)
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

			await this.validator.ValidateAndAddToModelStateAsync(request.Body, this.ModelState, cancellationToken);

			if (!this.ModelState.IsValid)
			{
				return this.BadRequest(
					v1.Responses.ErrorResponse.Create(Constants.Errors.Categories.ValidationError)
					.AddModelStateErrors(this.ModelState)
					);
			}

			var operationResults = this.binsService.FitBins(
				request.Body.Bins!, 
				request.Body.Items!,
				new Api.Models.FittingParameters
				{
					FindSmallestBinOnly = true,
					ReportFittedItems = false,
					ReportUnfittedItems = false
				}
			);

			return this.Ok(
				v1.Responses.QueryResponse.Create(request.Body.Bins!, request.Body.Items!, operationResults)
				);
		}
		catch (Exception ex)
		{
			this.logger.LogError(ex, "An exception occurred in {endpoint} endpoint", "Query by Custom");
			return this.InternalServerError(
				v1.Responses.ErrorResponse.Create(Constants.Errors.Categories.ServerError)
				.AddExceptionError(ex)
				);
		}
	}
}
