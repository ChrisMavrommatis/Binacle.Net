using Asp.Versioning;
using Binacle.Net.Api.Services;
using ChrisMavrommatis.Endpoints;
using ChrisMavrommatis.FluentValidation;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.Api.v2.Endpoints.Pack;

/// <summary>
/// Query by Custom endpoint
/// </summary>
[ApiVersion(v2.ApiVersion.Number)]
[Route("api/v{version:apiVersion}/[namespace]")]
public class ByCustom : EndpointWithRequest<v2.Requests.CustomPackRequestWithBody>
{
	private readonly IValidator<v2.Requests.CustomPackRequest> validator;
	private readonly ILockerService lockerService;
	private readonly ILogger<ByCustom> logger;

	/// <summary>
	/// Pack by Custom endpoint
	/// </summary>
	/// <param name="validator"></param>
	/// <param name="lockerService"></param>
	/// <param name="logger"></param>
	public ByCustom(
		IValidator<v2.Requests.CustomPackRequest> validator,
		ILockerService lockerService,
		ILogger<ByCustom> logger
	  )
	{
		this.validator = validator;
		this.lockerService = lockerService;
		this.logger = logger;
	}

	
	[HttpPost("by-custom")]
	[Consumes("application/json")]
	[Produces("application/json")]
	[MapToApiVersion(v2.ApiVersion.Number)]
	//[SwaggerRequestExample(typeof(CustomQueryRequest), typeof(CustomQueryRequestExample))]

	//[ProducesResponseType(typeof(Response<Bin?>), StatusCodes.Status200OK)]
	//[SwaggerResponseExample(typeof(Response<Bin?>), typeof(CustomQueryResponseExamples), StatusCodes.Status200OK)]

	//[ProducesResponseType(typeof(Response<List<IApiError>>), StatusCodes.Status400BadRequest)]
	//[SwaggerResponseExample(typeof(Response<List<IApiError>>), typeof(BadRequestErrorResponseExamples), StatusCodes.Status400BadRequest)]

	//[ProducesResponseType(typeof(Response<List<IApiError>>), StatusCodes.Status500InternalServerError)]
	//[SwaggerResponseExample(typeof(Response<List<IApiError>>), typeof(ServerErrorResponseExample), StatusCodes.Status500InternalServerError)]
	public override async Task<IActionResult> HandleAsync(v2.Requests.CustomPackRequestWithBody request, CancellationToken cancellationToken = default)
	{
		try
		{
			if (request is null || request.Body is null)
			{
				return this.BadRequest(
					v2.Responses.Response.ParameterError(nameof(request), Constants.Errors.Messages.MalformedRequestBody, Constants.Errors.Categories.RequestError)
					);
			}

			await this.validator.ValidateAndAddToModelStateAsync(request.Body, this.ModelState, cancellationToken);

			if (!this.ModelState.IsValid)
			{
				return this.BadRequest(
					v2.Responses.Response.ValidationError(this.ModelState, Constants.Errors.Categories.ValidationError)
					);
			}

			var operationResult = this.lockerService.PackBins(request.Body.Bins, request.Body.Items);

			return this.Ok(
				operationResult
				);
		}
		catch (Exception ex)
		{
			this.logger.LogError(ex, "An exception occurred in {endpoint} endpoint", "Pack by Custom");
			return this.InternalServerError(
				v2.Responses.Response.ExceptionError(ex, Constants.Errors.Categories.ServerError)
				);
		}
	}
}

