using Binacle.Net.Constants;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.Models;
using Binacle.Net.Services;
using Binacle.Net.v3.Models;
using Binacle.Net.v3.Requests;
using Binacle.Net.v3.Requests.Examples;
using Binacle.Net.v3.Responses;
using Binacle.Net.v3.Responses.Examples;
using FluentValidation;
using FluentValidation.Results;
using OpenApiExamples;

namespace Binacle.Net.v3.Endpoints.Pack;

internal class ByCustom : IGroupedEndpoint<ApiV3EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("pack/by-custom", HandleAsync)
			.WithTags("Pack")
			.WithSummary("Pack by Custom")
			.WithDescription("Pack items using custom bins")
			.Accepts<CustomPackRequest>("application/json")
			.RequestExample<CustomPackRequestExample>("application/json")
			.Produces<PackResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseExamples<CustomPackResponseExamples>(StatusCodes.Status200OK, "application/json")
			.WithResponseDescription(StatusCodes.Status200OK, ResponseDescription.ForPackResponse200OK)
			.Produces<ErrorResponse>(StatusCodes.Status400BadRequest, "application/json")
			.ResponseExamples<BadRequestErrorResponseExamples>(StatusCodes.Status400BadRequest, "application/json")
			.WithResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.RequireRateLimiting("Anonymous");
	}

	internal async Task<IResult> HandleAsync(
		LegacyValidatedBindingResult<CustomPackRequest?> request,
		IBinacleService binacleService,
		ILogger<ByCustom> logger,
		CancellationToken cancellationToken = default
	)
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Pack by Custom: v3");
		try
		{
			if (request.Value is null)
			{
				return Results.BadRequest(
					Response.ParameterError(
						nameof(request),
						request.Exception!.Message,
						ErrorCategory.RequestError
					)
				);
			}

			if (!(request.ValidationResult?.IsValid ?? false))
			{
				return Results.BadRequest(
					Response.ValidationError(request.ValidationResult!, ErrorCategory.ValidationError)
				);
			}

			var operationResults = await binacleService.PackBinsAsync(
				request.Value.Bins!,
				request.Value.Items!,
				new PackingParameters
				{
					Algorithm = request.Value.Parameters!.Algorithm!.Value
				}
			);

			using (var responseActivity = Diagnostics.ActivitySource.StartActivity("Create Response"))
			{
				return Results.Ok(
					PackResponse.Create(
						request.Value.Bins!,
						request.Value.Items!,
						request.Value.Parameters,
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

