using System.Net.Mime;
using Binacle.Net.Configuration;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.Services;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Fit;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v4.Endpoints.Fit;

internal class CustomSmallestBin : IGroupedEndpoint<ApiV4EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("fit/smallest-bin", HandleAsync)
			.WithOperationId("fit.customSmallestBin")
			.WithTags("Fit")
			.WithSummary("Fit the smallest custom bin")
			.WithDescription("Fit-check every custom bin and return the result for the smallest bin the items fit into.")

			.Accepts<FitCustomSmallestBinRequest>(MediaTypeNames.Application.Json)
			.RequestExample<FitCustomSmallestBinRequestExample>(MediaTypeNames.Application.Json)

			.Produces<FitBinResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
			.ResponseDescription(StatusCodes.Status200OK,
				"Returns the result of the fitting operation for the smallest custom bin the items fit into.")
			.ResponseExamples<FitCustomSmallestBinResponseExamples>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)

			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.ResponseExamples<Status400ResponseExamples>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)

			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For400BadRequest
			)
			.ResponseExamples<CustomBinsValidationProblemResponseExamples>(
				StatusCodes.Status422UnprocessableEntity,
				MediaTypeNames.Application.ProblemJson
			)
			.RequireRateLimiting("ApiUsage")
			.RequireCors(CorsPolicy.CoreApi);
	}

	internal static async Task<IResult> HandleAsync(
		BindingResult<FitCustomSmallestBinRequest> bindingResult,
		IBinacleService binacleService,
		ILogger<CustomSmallestBin> logger,
		CancellationToken cancellationToken = default
	)
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Fit Custom Smallest Bin: v4");

		return await bindingResult.ValidateAsync(async request =>
		{
			var algorithm = request.Parameters.GetAlgorithm();

			OperationResult result = null!;
			if (algorithm.HasValue)
			{
				result = await binacleService.SmallestBinAsync(
					algorithm.Value,
					request.Bins,
					request.Items,
					request.Parameters.ForFittingOperation(),
					cancellationToken
				);
			}
			else
			{
				result = await binacleService.SmallestBinAsync(
					request.Bins,
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
