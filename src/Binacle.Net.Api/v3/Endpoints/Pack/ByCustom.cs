using Asp.Versioning;
using Binacle.Net.Api.Models;
using Binacle.Net.Api.Services;
using ChrisMavrommatis.Endpoints;
using ChrisMavrommatis.FluentValidation;
using ChrisMavrommatis.SwaggerExamples.Attributes;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.Api.v3.Endpoints.Pack;

/// <summary>
/// Pack by Custom endpoint
/// </summary>
[ApiVersion(v3.ApiVersion.Number, Deprecated = v3.ApiVersion.IsDeprecated)]
[Route("api/v{version:apiVersion}/[namespace]")]
public class ByCustom : EndpointWithRequest<v3.Requests.CustomPackRequestWithBody>
{
	private readonly IValidator<v3.Requests.CustomPackRequest> validator;
	private readonly IBinsService binsService;
	private readonly ILogger<ByCustom> logger;

	/// <summary>
	/// Pack by Custom endpoint
	/// </summary>
	/// <param name="validator"></param>
	/// <param name="binsService"></param>
	/// <param name="logger"></param>
	public ByCustom(
		IValidator<v3.Requests.CustomPackRequest> validator,
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
	[MapToApiVersion(v3.ApiVersion.Number)]
	[SwaggerRequestExample(typeof(v3.Requests.CustomPackRequest), typeof(v3.Requests.Examples.CustomPackRequestExample))]

	[ProducesResponseType(typeof(v3.Responses.PackResponse), StatusCodes.Status200OK)]
	[SwaggerResponseExample(typeof(v3.Responses.PackResponse), typeof(v3.Responses.Examples.CustomPackResponseExamples), StatusCodes.Status200OK)]

	[ProducesResponseType(typeof(v3.Responses.ErrorResponse), StatusCodes.Status400BadRequest)]
	[SwaggerResponseExample(typeof(v3.Responses.ErrorResponse), typeof(v3.Responses.Examples.BadRequestErrorResponseExamples), StatusCodes.Status400BadRequest)]

	[ProducesResponseType(typeof(v3.Responses.ErrorResponse), StatusCodes.Status500InternalServerError)]
	[SwaggerResponseExample(typeof(v3.Responses.ErrorResponse), typeof(v3.Responses.Examples.ServerErrorResponseExample), StatusCodes.Status500InternalServerError)]
	public override async Task<IActionResult> HandleAsync(v3.Requests.CustomPackRequestWithBody request, CancellationToken cancellationToken = default)
	{
		try
		{
			if (request is null || request.Body is null)
			{
				return this.BadRequest(
					Models.Response.ParameterError(nameof(request), Constants.Errors.Messages.MalformedRequestBody,
						Constants.Errors.Categories.RequestError)
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
					Algorithm = request.Body.Parameters!.Algorithm!.Value,
					StopAtSmallestBin = false,
					NeverReportUnpackedItems = false,
					OptInToEarlyFails = false,
					ReportPackedItemsOnlyWhenFullyPacked = false,
				}
			);

			return this.Ok(
				v3.Responses.PackResponse.Create(
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
