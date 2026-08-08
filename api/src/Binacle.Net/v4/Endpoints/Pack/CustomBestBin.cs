using Binacle.Lib.Abstractions.Models;
using Binacle.Net.Configuration;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.Services;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Pack;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v4.Endpoints.Pack;

internal class CustomBestBin : IGroupedEndpoint<ApiV4EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("pack/best-bin", HandleAsync)
			.WithOperationId("pack.customBestBin")
			.WithTags("Pack")
			.WithSummary("Pack the best custom bin")
			.WithDescription("Pack every custom bin and return the result for the bin the items fill the most.")

			.Accepts<PackCustomBestBinRequest>("application/json")
			.RequestExample<PackCustomBestBinRequestExample>("application/json")

			.Produces<PackBinResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseDescription(StatusCodes.Status200OK,
				"Returns the result of the packing operation for the custom bin with the highest utilization.")
			.ResponseExamples<PackCustomBestBinResponseExamples>(StatusCodes.Status200OK, "application/json")

			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.ResponseExamples<Status400ResponseExamples>(StatusCodes.Status400BadRequest, "application/problem+json")

			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For400BadRequest
			)
			.ResponseExamples<CustomBinsValidationProblemResponseExamples>(
				StatusCodes.Status422UnprocessableEntity,
				"application/problem+json"
			)
			.RequireRateLimiting("ApiUsage")
			.RequireCors(CorsPolicy.CoreApi);
	}

	internal async Task<IResult> HandleAsync(
		BindingResult<PackCustomBestBinRequest> bindingResult,
		IBinacleService binacleService,
		ILogger<CustomBestBin> logger,
		CancellationToken cancellationToken = default
	)
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Pack Custom Best Bin: v4");

		return await bindingResult.ValidateAsync(async request =>
		{
			var algorithm = request.Parameters.GetAlgorithm();

			OperationResult result = null!;
			if (algorithm.HasValue)
			{
				result = await binacleService.BestBinAsync(
					algorithm.Value,
					request.Bins,
					request.Items,
					request.Parameters.ForPackingOperation(),
					cancellationToken
				);
			}
			else
			{
				result = await binacleService.BestBinAsync(
					request.Bins,
					request.Items,
					request.Parameters.ForPackingOperation(),
					cancellationToken
				);
			}

			using (var responseActivity = Diagnostics.ActivitySource.StartActivity("Create Response"))
			{
				return Results.Ok(
					PackBinResponse.From(
						request.Parameters,
						result
					)
				);
			}
		});
	}
}
