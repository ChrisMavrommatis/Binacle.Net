using Binacle.Lib.Abstractions.Models;
using Binacle.Net.Configuration;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.Services;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Fit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v4.Endpoints.Fit;

internal class PresetCompare : IGroupedEndpoint<ApiV4EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("fit/compare-bins/{preset}", HandleAsync)
			.WithTags("Fit")
			.WithSummary("Compare the bins in a preset")
			.WithDescription("Fit-check every bin in a preset and return the result for each one.")

			.Accepts<FitPresetCompareRequest>("application/json")
			.RequestExample<FitPresetCompareRequestExample>("application/json")

			.Produces<FitCompareResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseDescription(StatusCodes.Status200OK,
				"Returns the result of the fitting operation for every bin in the preset.")
			.ResponseExamples<FitPresetCompareResponseExamples>(StatusCodes.Status200OK, "application/json")

			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.ResponseExamples<Status400ResponseExamples>(StatusCodes.Status400BadRequest, "application/problem+json")

			.Produces(StatusCodes.Status404NotFound)
			.ResponseDescription(StatusCodes.Status404NotFound, "If the preset does not exist.")

			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For400BadRequest
			)
			.ResponseExamples<PresetBinsValidationProblemResponseExamples>(
				StatusCodes.Status422UnprocessableEntity,
				"application/problem+json"
			)
			.RequireRateLimiting("ApiUsage")
			.RequireCors(CorsPolicy.CoreApi);
	}

	internal async Task<IResult> HandleAsync(
		[FromRoute] string preset,
		BindingResult<FitPresetCompareRequest> bindingResult,
		IOptions<BinPresetOptions> presetOptions,
		IBinacleService binacleService,
		ILogger<PresetCompare> logger,
		CancellationToken cancellationToken = default
	)
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Fit Preset Compare: v4");

		return await bindingResult.ValidateAsync(async request =>
		{
			if (!presetOptions.Value.TryGetPreset(preset, out var presetOption))
			{
				return Results.NotFound(null);
			}

			var algorithm = request.Parameters.GetAlgorithm();

			IDictionary<string, OperationResult> results = null!;
			if (algorithm.HasValue)
			{
				results = await binacleService.MultipleBinsAsync(
					algorithm.Value,
					presetOption.Bins,
					request.Items!,
					request.Parameters.ForFittingOperation(),
					cancellationToken
				);
			}
			else
			{
				results = await binacleService.MultipleBinsAsync(
					presetOption.Bins,
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
						presetOption.Bins,
						results
					)
				);
			}
		});
	}
}
