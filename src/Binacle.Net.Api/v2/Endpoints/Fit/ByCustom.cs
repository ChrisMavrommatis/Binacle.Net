using Asp.Versioning;
using Binacle.Net.Api.Models;
using Binacle.Net.Api.Services;
using ChrisMavrommatis.Endpoints;
using ChrisMavrommatis.FluentValidation;
using ChrisMavrommatis.SwaggerExamples.Attributes;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.Api.v2.Endpoints.Fit;


/// <summary>
/// Fit by Custom endpoint
/// </summary>
[ApiVersion(v2.ApiVersion.Number, Deprecated = v2.ApiVersion.IsDeprecated)]
[Route("api/v{version:apiVersion}/[namespace]")]
public class ByCustom : EndpointWithRequest<v2.Requests.CustomFitRequestWithBody>
{
	private readonly IValidator<v2.Requests.CustomFitRequest> validator;
	private readonly ILockerService lockerService;
	private readonly ILogger<ByCustom> logger;

	/// <summary>
	/// Fit by Custom endpoint
	/// </summary>
	/// <param name="validator"></param>
	/// <param name="lockerService"></param>
	/// <param name="logger"></param>
	public ByCustom(
		IValidator<v2.Requests.CustomFitRequest> validator,
		ILockerService lockerService,
		ILogger<ByCustom> logger
	  )
	{
		this.validator = validator;
		this.lockerService = lockerService;
		this.logger = logger;
	}

	/// <summary>
	/// Perform a bin fitting function using custom bins.
	/// </summary>
	/// <returns>An array of results indicating if a bin can accommodate all of the items</returns>
	/// <remarks>
	/// Example request:
	///     
	///     POST /api/v2/fit/by-custom
	///		{
	///			"parameters": {
	///				"reportFittedItems": true,	
	///				"reportUnfittedItems": true,
	///				"findSmallestBinOnly": false
	///			},
	///			"bins" : [
	///				{
	///					"id": "custom_bin_1",
	///					"length": 10,
	///					"width": 40,
	///					"height": 60
	///				},
	///				{
	///					"id": "custom_bin_2",
	///					"length": 20,
	///					"width": 40,
	///					"height": 60
	///				}
	///			],
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
	[SwaggerRequestExample(typeof(v2.Requests.CustomFitRequest), typeof(v2.Requests.Examples.CustomFitRequestExample))]

	[ProducesResponseType(typeof(v2.Responses.FitResponse), StatusCodes.Status200OK)]
	[SwaggerResponseExample(typeof(v2.Responses.FitResponse), typeof(v2.Responses.Examples.CustomFitResponseExamples), StatusCodes.Status200OK)]

	[ProducesResponseType(typeof(v2.Responses.ErrorResponse), StatusCodes.Status400BadRequest)]
	[SwaggerResponseExample(typeof(v2.Responses.ErrorResponse), typeof(v2.Responses.Examples.BadRequestErrorResponseExamples), StatusCodes.Status400BadRequest)]

	[ProducesResponseType(typeof(v2.Responses.ErrorResponse), StatusCodes.Status500InternalServerError)]
	[SwaggerResponseExample(typeof(v2.Responses.ErrorResponse), typeof(v2.Responses.Examples.ServerErrorResponseExample), StatusCodes.Status500InternalServerError)]
	public override async Task<IActionResult> HandleAsync(v2.Requests.CustomFitRequestWithBody request, CancellationToken cancellationToken = default)
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

			var operationResults = this.lockerService.FitBins(
				request.Body.Bins!, 
				request.Body.Items!,
				new FittingParameters
				{
					FindSmallestBinOnly = request.Body.Parameters?.FindSmallestBinOnly ?? true,
					ReportFittedItems = request.Body.Parameters?.ReportFittedItems ?? false,
					ReportUnfittedItems = request.Body.Parameters?.ReportUnfittedItems ?? false,
				}
			);

			return this.Ok(
				v2.Responses.FitResponse.Create(
					request.Body.Bins!, 
					request.Body.Items!, 
					request.Body.Parameters,
					operationResults
				)
			);
		}
		catch (Exception ex)
		{
			this.logger.LogError(ex, "An exception occurred in {endpoint} endpoint", "Query by Custom");
			return this.InternalServerError(
				Models.Response.ExceptionError(ex, Constants.Errors.Categories.ServerError)
				);
		}
	}
}
