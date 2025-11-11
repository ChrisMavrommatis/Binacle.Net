using Binacle.Net.Configuration;
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
			
			.Accepts<PackByPresetRequest>("application/json")
			.RequestExample<PackByPresetRequestExample>("application/json")
			
			.Produces<PackResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseExamples<PackByPresetResponseExamples>(StatusCodes.Status200OK, "application/json")
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
			.ResponseExamples<PackByPresetValidationProblemExamples>(
				StatusCodes.Status422UnprocessableEntity,
				"application/problem+json"
			)
			.RequireRateLimiting("ApiUsage")
			.RequireCors(CorsPolicy.CoreApi);
	}

	internal async Task<IResult> HandleAsync(
		[FromRoute] string preset,
		BindingResult<PackByPresetRequest> bindingResult,
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
					OptInToEarlyFails = false,
					ReportPackedItemsOnlyWhenFullyPacked = false,
					NeverReportUnpackedItems = false
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

