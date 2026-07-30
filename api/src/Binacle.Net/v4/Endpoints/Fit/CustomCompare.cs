using Binacle.Lib.Abstractions.Models;
using Binacle.Net.Configuration;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.Services;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Fit;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v4.Endpoints.Fit;

internal class CustomCompare : IGroupedEndpoint<ApiV4EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("fit/compare-bins", HandleAsync)
			.WithOperationId("fit.customCompareBins")
			.WithTags("Fit")
			.WithSummary("Compare custom bins")
			.WithDescription("Fit-check every custom bin and return the result for each one.")

			.Accepts<FitCustomCompareRequest>("application/json")
			.RequestExample<FitCustomCompareRequestExample>("application/json")

			.Produces<FitCompareResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseDescription(StatusCodes.Status200OK,
				"Returns the result of the fitting operation for every custom bin, in the order they were sent.")
			.ResponseExamples<FitCustomCompareResponseExamples>(StatusCodes.Status200OK, "application/json")

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
		BindingResult<FitCustomCompareRequest> bindingResult,
		IBinacleService binacleService,
		ILogger<CustomCompare> logger,
		CancellationToken cancellationToken = default
	)
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Fit Custom Compare: v4");

		return await bindingResult.ValidateAsync(async request =>
		{
			var algorithm = request.Parameters.GetAlgorithm();

			IDictionary<string, OperationResult> results = null!;
			if (algorithm.HasValue)
			{
				results = await binacleService.MultipleBinsAsync(
					algorithm.Value,
					request.Bins!,
					request.Items!,
					request.Parameters.ForFittingOperation(),
					cancellationToken
				);
			}
			else
			{
				results = await binacleService.MultipleBinsAsync(
					request.Bins!,
					request.Items!,
					request.Parameters.ForFittingOperation(),
					cancellationToken
				);
			}

			using (var responseActivity = Diagnostics.ActivitySource.StartActivity("Create Response"))
			{
				return Results.Ok(
					FitCompareResponse.From(
						request.Parameters,
						request.Bins!,
						results
					)
				);
			}
		});
	}
}
