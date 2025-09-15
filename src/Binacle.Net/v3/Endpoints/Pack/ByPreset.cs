using Binacle.Net.Configuration.Models;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.Models;
using Binacle.Net.Services;
using Binacle.Net.v3.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v3.Endpoints.Pack;

internal class ByPreset : IGroupedEndpoint<ApiV3EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("pack/by-preset/{preset}", HandleAsync)
			.WithTags("Pack")
			.WithSummary("Pack by Preset")
			.WithDescription("Pack items using a specified bin preset.")
			
			.Accepts<PresetPackRequest>("application/json")
			.RequestExample<PresetPackRequestExample>("application/json")
			
			.Produces<PackResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseExamples<PresetPackResponseExamples>(StatusCodes.Status200OK, "application/json")
			.ResponseDescription(StatusCodes.Status200OK, ResponseDescription.ForPackResponse200Ok)
			
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.ResponseExamples<Status400ResponseExamples>(StatusCodes.Status400BadRequest, "application/problem+json")
			
			.Produces(StatusCodes.Status404NotFound)
			.ResponseDescription(StatusCodes.Status404NotFound, ResponseDescription.ForPreset404NotFound)
			
			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For400BadRequest
			)
			.ResponseExamples<PresetPackValidationProblemExamples>(
				StatusCodes.Status422UnprocessableEntity,
				"application/problem+json"
			)
			.RequireRateLimiting("ApiUsage");
	}

	internal async Task<IResult> HandleAsync(
		[FromRoute] string preset,
		BindingResult<PresetPackRequest> bindingResult,
		IOptions<BinPresetOptions> presetOptions,
		IBinacleService binacleService,
		ILogger<ByPreset> logger,
		CancellationToken cancellationToken = default
	)
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Pack by Preset: v3");
		
		return await bindingResult.ValidateAsync(async request =>
		{
			if (!presetOptions.Value.Presets.TryGetValue(preset, out var presetOption))
			{
				return Results.NotFound(null);
			}
			
			var operationResults = await binacleService.PackBinsAsync(
				presetOption.Bins!,
				request.Items!,
				new PackingParameters
				{
					Algorithm = request.Parameters!.Algorithm!.Value,
					OptInToEarlyFails = request.Parameters.OptInToEarlyFails,
					ReportPackedItemsOnlyWhenFullyPacked = request.Parameters.ReportPackedItemsOnlyWhenFullyPacked,
					NeverReportUnpackedItems = request.Parameters.NeverReportUnpackedItems
				}
			);

			using (var responseActivity = Diagnostics.ActivitySource.StartActivity("Create Response"))
			{
				return Results.Ok(
					PackResponse.Create(
						presetOption.Bins!,
						request.Items!,
						request.Parameters,
						operationResults
					)
				);
			}
		});
	}
}

