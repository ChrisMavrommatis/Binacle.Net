using System.ComponentModel;
using System.Net.Mime;
using Binacle.Net.Configuration;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.Services;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Pack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v4.Endpoints.Pack;

internal class PresetCompare : IGroupedEndpoint<ApiV4EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("pack/compare-bins/{preset}", HandleAsync)
			.WithOperationId("pack.presetCompareBins")
			.WithTags("Pack")
			.WithSummary("Compare the bins in a preset")
			.WithDescription("Pack every bin in a preset and return the result for each one.")

			.Accepts<PackPresetCompareRequest>(MediaTypeNames.Application.Json)
			.RequestExample<PackPresetCompareRequestExample>(MediaTypeNames.Application.Json)

			.Produces<PackCompareResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
			.ResponseDescription(StatusCodes.Status200OK,
				"Returns the result of the packing operation for every bin in the preset.")
			.ResponseExamples<PackPresetCompareResponseExamples>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)

			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.ResponseExamples<Status400ResponseExamples>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)

			.Produces(StatusCodes.Status404NotFound)
			.ResponseDescription(StatusCodes.Status404NotFound, "If the preset does not exist.")

			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For400BadRequest
			)
			.ResponseExamples<PresetBinsValidationProblemResponseExamples>(
				StatusCodes.Status422UnprocessableEntity,
				MediaTypeNames.Application.ProblemJson
			)
			.RateLimited()
			.RequireCors(CorsPolicy.CoreApi);
	}

	internal static async Task<IResult> HandleAsync(
		[FromRoute][Description(SchemaDescriptions.PresetParam)] string preset,
		BindingResult<PackPresetCompareRequest> bindingResult,
		IOptions<BinPresetOptions> presetOptions,
		IBinacleService binacleService,
		ILogger<PresetCompare> logger,
		CancellationToken cancellationToken = default
	)
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Pack Preset Compare: v4");

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
					request.Items,
					request.Parameters.ForPackingOperation(),
					cancellationToken
				);
			}
			else
			{
				results = await binacleService.MultipleBinsAsync(
					presetOption.Bins,
					request.Items,
					request.Parameters.ForPackingOperation(),
					cancellationToken
				);
			}

			using (var responseActivity = Diagnostics.ActivitySource.StartActivity("Create Response"))
			{
				return Results.Ok(
					PackCompareResponse.From(
						request.Parameters,
						presetOption.Bins,
						results
					)
				);
			}
		});
	}
}
