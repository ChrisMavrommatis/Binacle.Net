using System.Net.Mime;
using Binacle.Net.Configuration;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.Services;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Fit;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v4.Endpoints.Fit;

internal class CustomBin : IGroupedEndpoint<ApiV4EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("fit/bin", HandleAsync)
			.WithOperationId("fit.customBin")
			.WithTags("Fit")
			.WithSummary("Fit a custom bin")
			.WithDescription("Attempt to find if all items fit into a custom bin.")
			
			.Accepts<FitCustomBinRequest>(MediaTypeNames.Application.Json)
			.RequestExample<FitCustomBinRequestExample>(MediaTypeNames.Application.Json)
			
			.Produces<FitBinResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
			.ResponseDescription(StatusCodes.Status200OK, 
				"Returns the result of the fitting operation for the specified custom bin and items.")
			.ResponseExamples<FitCustomBinResponseExamples>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
			
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.ResponseExamples<Status400ResponseExamples>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)
			
			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For400BadRequest
			)
			.ResponseExamples<CustomBinValidationProblemResponseExamples>(
				StatusCodes.Status422UnprocessableEntity,
			 MediaTypeNames.Application.ProblemJson
			)
			.RequireRateLimiting("ApiUsage")
			.RequireCors(CorsPolicy.CoreApi);
	}

	internal static async Task<IResult> HandleAsync(
		BindingResult<FitCustomBinRequest> bindingResult,
		IBinacleService binacleService,
		ILogger<CustomBin> logger,
		CancellationToken cancellationToken = default
	)
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Fit Custom Bin: v4");
		
		return await bindingResult.ValidateAsync(async request =>
		{
			var algorithm = request.Parameters.GetAlgorithm();
			
			OperationResult result = null!;
			if (algorithm.HasValue)
			{
				result = await binacleService.SingleBinAsync(
					algorithm.Value,
					request.Bin,
					request.Items,
					request.Parameters.ForFittingOperation(),
					cancellationToken
				);
			}
			else
			{
				result = await binacleService.SingleBinAsync(
					request.Bin,
					request.Items,
					request.Parameters.ForFittingOperation(),
					cancellationToken
				);
			}
			
			using (var responseActivity = Diagnostics.ActivitySource.StartActivity("Create Response"))
			{
				return Results.Ok(
					FitBinResponse.From(
						request.Parameters,
						result
					)
				);
			}
		});
	}
}

