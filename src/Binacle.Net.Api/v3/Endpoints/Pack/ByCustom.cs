using Binacle.Net.Api.Constants;
using Binacle.Net.Api.Kernel.Endpoints;
using Binacle.Net.Api.Models;
using Binacle.Net.Api.Services;
using Binacle.Net.Api.v3.Models;
using Binacle.Net.Api.v3.Requests;
using Binacle.Net.Api.v3.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.Api.v3.Endpoints.Pack;

internal class ByCustom : IGroupedEndpoint<ApiV3EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("pack/by-custom", HandleAsync)
			.WithTags("Pack")
			.WithSummary("Pack by Custom")
			.WithDescription("Pack items using custom bins")
			.Accepts<CustomPackRequest>("application/json")
			.Produces<PackResponse>(StatusCodes.Status200OK, "application/json")
			.WithResponseDescription(StatusCodes.Status200OK, ResponseDescription.ForPackResponse200OK)
			.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.WithResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest);
		// [SwaggerRequestExample(typeof(v3.Requests.CustomPackRequest), typeof(v3.Requests.Examples.CustomPackRequestExample))]
		// [SwaggerResponseExample(typeof(v3.Responses.PackResponse), typeof(v3.Responses.Examples.CustomPackResponseExamples), StatusCodes.Status200OK)]
		// [SwaggerResponseExample(typeof(v3.Responses.ErrorResponse), typeof(v3.Responses.Examples.BadRequestErrorResponseExamples), StatusCodes.Status400BadRequest)]
		// [SwaggerResponseExample(typeof(v3.Responses.ErrorResponse), typeof(v3.Responses.Examples.ServerErrorResponseExample), StatusCodes.Status500InternalServerError)]
	}

	internal async Task<IResult> HandleAsync(
		[FromBody] CustomPackRequest? request,
		IValidator<CustomPackRequest> validator,
		IBinacleService binacleService,
		ILogger<ByCustom> logger,
		CancellationToken cancellationToken = default
	)
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Pack by Custom: v3");
		try
		{
			if (request is null)
			{
				return Results.BadRequest(
					Response.ParameterError(
						nameof(request),
						ErrorMessage.MalformedRequestBody,
						ErrorCategory.RequestError
					)
				);
			}

			// using(var validationActivity = Diagnostics.ActivitySource.StartActivity("Validate Request"))
			var validationResult = await validator.ValidateAsync(request, cancellationToken);

			if (!validationResult.IsValid)
			{
				return Results.BadRequest(
					Response.ValidationError(validationResult, ErrorCategory.ValidationError)
				);
			}

			var operationResults = await binacleService.PackBinsAsync(
				request.Bins!,
				request.Items!,
				new PackingParameters
				{
					Algorithm = request.Parameters!.Algorithm!.Value
				}
			);

			using (var responseActivity = Diagnostics.ActivitySource.StartActivity("Create Response"))
			{
				return Results.Ok(
					PackResponse.Create(
						request.Bins!,
						request.Items!,
						request.Parameters,
						operationResults
					)
				);
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "An exception occurred in {endpoint} endpoint", "Pack by Custom");
			return Results.InternalServerError(
				Response.ExceptionError(ex, ErrorCategory.ServerError)
			);
		}
	}
}
