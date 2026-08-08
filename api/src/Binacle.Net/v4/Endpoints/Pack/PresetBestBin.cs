using System.ComponentModel;
using Binacle.Lib.Abstractions.Models;
using Binacle.Net.Configuration;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.Services;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Pack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v4.Endpoints.Pack;

internal class PresetBestBin : IGroupedEndpoint<ApiV4EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("pack/best-bin/{preset}", HandleAsync)
			.WithOperationId("pack.presetBestBin")
			.WithTags("Pack")
			.WithSummary("Pack the best bin in a preset")
			.WithDescription("Pack every bin in a preset and return the result for the bin the items fill the most.")

			.Accepts<PackPresetBestBinRequest>("application/json")
			.RequestExample<PackPresetBestBinRequestExample>("application/json")

			.Produces<PackBinResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseDescription(StatusCodes.Status200OK,
				"Returns the result of the packing operation for the bin in the preset with the highest utilization.")
			.ResponseExamples<PackPresetBestBinResponseExamples>(StatusCodes.Status200OK, "application/json")

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
		[FromRoute][Description(SchemaDescriptions.PresetParam)] string preset,
		BindingResult<PackPresetBestBinRequest> bindingResult,
		IOptions<BinPresetOptions> presetOptions,
		IBinacleService binacleService,
		ILogger<PresetBestBin> logger,
		CancellationToken cancellationToken = default
	)
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Pack Preset Best Bin: v4");

		return await bindingResult.ValidateAsync(async request =>
		{
			if (!presetOptions.Value.TryGetPreset(preset, out var presetOption))
			{
				return Results.NotFound(null);
			}

			var algorithm = request.Parameters.GetAlgorithm();

			OperationResult result = null!;
			if (algorithm.HasValue)
			{
				result = await binacleService.BestBinAsync(
					algorithm.Value,
					presetOption.Bins,
					request.Items,
					request.Parameters.ForPackingOperation(),
					cancellationToken
				);
			}
			else
			{
				result = await binacleService.BestBinAsync(
					presetOption.Bins,
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
