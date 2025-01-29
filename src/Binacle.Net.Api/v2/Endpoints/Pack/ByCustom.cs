using Asp.Versioning;
using Binacle.Net.Api.Models;
using Binacle.Net.Api.Services;
using ChrisMavrommatis.Endpoints;
using ChrisMavrommatis.FluentValidation;
using ChrisMavrommatis.SwaggerExamples.Attributes;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.Api.v2.Endpoints.Pack;

/// <summary>
/// Pack by Custom endpoint
/// </summary>
[ApiVersion(v2.ApiVersion.Number, Deprecated = v2.ApiVersion.IsDeprecated)]
[Route("api/v{version:apiVersion}/[namespace]")]
public class ByCustom : EndpointWithRequest<v2.Requests.CustomPackRequestWithBody>
{
	private readonly IValidator<v2.Requests.CustomPackRequest> validator;
	private readonly IBinsService binsService;
	private readonly ILogger<ByCustom> logger;

	/// <summary>
	/// Pack by Custom endpoint
	/// </summary>
	/// <param name="validator"></param>
	/// <param name="binsService"></param>
	/// <param name="logger"></param>
	public ByCustom(
		IValidator<v2.Requests.CustomPackRequest> validator,
		IBinsService binsService,
		ILogger<ByCustom> logger
	  )
	{
		this.validator = validator;
		this.binsService = binsService;
		this.logger = logger;
	}

	/// <summary>
	/// Pack items using custom bins.
	/// </summary>
	/// <returns>An array of results indicating the result per bin</returns>
	/// <remarks>
	/// Example request:
	///     
	///     POST /api/v2/pack/by-custom
	///		{
	///			"parameters": {
	///				"neverReportUnpackedItems": false,
	///				"optInToEarlyFails": false,
	///				"stopAtSmallestBin": false,
	///				"reportPackedItemsOnlyWhenFullyPacked": false
	///			},
	///			"bins": [
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
	[HttpPost("by-custom")]
	[Consumes("application/json")]
	[Produces("application/json")]
	[MapToApiVersion(v2.ApiVersion.Number)]
	[SwaggerRequestExample(typeof(v2.Requests.CustomPackRequest), typeof(v2.Requests.Examples.CustomPackRequestExample))]

	[ProducesResponseType(typeof(v2.Responses.PackResponse), StatusCodes.Status200OK)]
	[SwaggerResponseExample(typeof(v2.Responses.PackResponse), typeof(v2.Responses.Examples.CustomPackResponseExamples), StatusCodes.Status200OK)]

	[ProducesResponseType(typeof(v2.Responses.ErrorResponse), StatusCodes.Status400BadRequest)]
	[SwaggerResponseExample(typeof(v2.Responses.ErrorResponse), typeof(v2.Responses.Examples.BadRequestErrorResponseExamples), StatusCodes.Status400BadRequest)]

	[ProducesResponseType(typeof(v2.Responses.ErrorResponse), StatusCodes.Status500InternalServerError)]
	[SwaggerResponseExample(typeof(v2.Responses.ErrorResponse), typeof(v2.Responses.Examples.ServerErrorResponseExample), StatusCodes.Status500InternalServerError)]
	public override async Task<IActionResult> HandleAsync(v2.Requests.CustomPackRequestWithBody request, CancellationToken cancellationToken = default)
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

			var operationResults = this.binsService.PackBins(
				request.Body.Bins!,
				request.Body.Items!,
				new Api.Models.PackingParameters
				{
					Algorithm = Algorithm.FFD,
					StopAtSmallestBin = request.Body.Parameters?.StopAtSmallestBin ?? false,
					NeverReportUnpackedItems = request.Body.Parameters?.NeverReportUnpackedItems ?? false,
					OptInToEarlyFails = request.Body.Parameters?.OptInToEarlyFails ?? false,
					ReportPackedItemsOnlyWhenFullyPacked = request.Body.Parameters?.ReportPackedItemsOnlyWhenFullyPacked ?? false
				}
			);

			return this.Ok(
				v2.Responses.PackResponse.Create(
					request.Body.Bins!,
					request.Body.Items!,
					request.Body.Parameters,
					operationResults
				)
			);

		}
		catch (Exception ex)
		{
			this.logger.LogError(ex, "An exception occurred in {endpoint} endpoint", "Pack by Custom");
			return this.InternalServerError(
				Models.Response.ExceptionError(ex, Constants.Errors.Categories.ServerError)
				);
		}
	}
}

