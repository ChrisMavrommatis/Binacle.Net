using Binacle.Net.Configuration;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.v3.Contracts;
using Binacle.Net.Services;
using Binacle.Net.v3.ExtensionMethods;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v3.Endpoints.Fit;

internal class ByCustom: IGroupedEndpoint<ApiV3EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("fit/by-custom", HandleAsync)
			.WithOperationId("fitByCustom")
			.WithTags("Fit")
			.WithSummary("Fit by custom")
			.WithDescription("Perform a bin fitting function using custom bins.")
			
			.Accepts<FitByCustomRequest>("application/json")
			.RequestExample<FitByCustomRequestExample>("application/json")
			
			.Produces<FitResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseDescription(StatusCodes.Status200OK, ResponseDescription.ForFitResponse200Ok)
			.ResponseExamples<FitByCustomResponseExamples>(StatusCodes.Status200OK, "application/json")
			
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.ResponseExamples<Status400ResponseExamples>(StatusCodes.Status400BadRequest, "application/problem+json")
			
			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For400BadRequest
			)
			.ResponseExamples<FitByCustomValidationProblemExamples>(
				StatusCodes.Status422UnprocessableEntity,
				"application/problem+json"
			)
			.RequireRateLimiting("ApiUsage")
			.RequireCors(CorsPolicy.CoreApi);
	}

	internal async Task<IResult> HandleAsync(
		BindingResult<FitByCustomRequest> bindingResult,
		IBinacleService binacleService,
		ILogger<ByCustom> logger,
		CancellationToken cancellationToken = default
	)
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Fit by Custom: v3");
		
		return await bindingResult.ValidateAsync(async request =>
		{
			var operationResults = await binacleService.MultipleBinsAsync(
				request.Parameters!.Algorithm.ToLibAlgorithm(),
				request.Bins!,
				request.Items!,
				request.Parameters!,
				cancellationToken
			);

			using (var responseActivity = Diagnostics.ActivitySource.StartActivity("Create Response"))
			{
				return Results.Ok(
					FitResponse.Create(
						request.Bins!,
						request.Items!,
						request.Parameters,
						operationResults
					)
				);
			}
		});
	}
}


