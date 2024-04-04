﻿using Asp.Versioning;
using Binacle.Net.Api.Models.Requests;
using Binacle.Net.Api.Models.Requests.Examples;
using Binacle.Net.Api.Models.Responses;
using Binacle.Net.Api.Models.Responses.Examples;
using Binacle.Net.Api.Services;
using ChrisMavrommatis.Endpoints;
using ChrisMavrommatis.FluentValidation;
using ChrisMavrommatis.SwaggerExamples.Attributes;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.Api.Endpoints.Query;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[namespace]")]
public class ByCustom : EndpointWithRequest<CustomQueryRequestWithBody>
{
	private readonly IValidator<CustomQueryRequest> customQueryRequestValidator;
	private readonly ILockerService lockerService;

	public ByCustom(
		IValidator<CustomQueryRequest> customQueryRequestValidator,
		ILockerService lockerService
	  )
	{
		this.customQueryRequestValidator = customQueryRequestValidator;
		this.lockerService = lockerService;
	}

	/// <summary>
	/// Perform a bin fit query using custom bins.
	/// </summary>
	/// <returns>The bin that fits all of the items</returns>
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
	/// <response code="200">Returns the bin that fits all of the items</response>
	/// <response code="400">If the request is invalid</response>
	/// <response code="500">If an unexpected error occurs</response>
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

			await this.customQueryRequestValidator.ValidateAndAddToModelStateAsync(request.Body, this.ModelState, cancellationToken);

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
			return this.InternalServerError(
				ErrorResponse.Create(Constants.Errors.Categories.ServerError)
				.AddExceptionError(ex)
				);
		}
	}
}
