using Binacle.Net.Configuration;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.Models;
using Binacle.Net.Services;
using Binacle.Net.v3.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v3.Endpoints.Fit;

internal class ByPreset : IGroupedEndpoint<ApiV3EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("fit/by-preset/{preset}", HandleAsync)
			.WithTags("Fit")
			.WithSummary("Fit by Preset")
			.WithDescription("Perform a bin fit function using a specified bin preset.")
			
			.Accepts<FitByPresetRequest>("application/json")
			.RequestExample<FitByPresetRequestExample>("application/json")
			
			.Produces<FitResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseExamples<FitByPresetResponseExamples>(StatusCodes.Status200OK, "application/json")
			.ResponseDescription(StatusCodes.Status200OK, ResponseDescription.ForFitResponse200Ok)
			
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
			.ResponseExamples<FitByPresetValidationProblemExamples>(
				StatusCodes.Status422UnprocessableEntity,
				"application/problem+json"
			)
			.RequireRateLimiting("ApiUsage")
			.RequireCors(CorsPolicy.CoreApi);
	}

	internal async Task<IResult> HandleAsync(
		[FromRoute] string preset,
		BindingResult<FitByPresetRequest> bindingResult,
		IOptions<BinPresetOptions> presetOptions,
		IBinacleService binacleService,
		ILogger<ByPreset> logger,
		CancellationToken cancellationToken = default
	)
	{
		using var activity = Diagnostics.ActivitySource.StartActivity("Fit by Preset: v3");
		
		return await bindingResult.ValidateAsync(async request =>
		{
			if (!presetOptions.Value.Presets.TryGetValue(preset, out var presetOption))
			{
				return Results.NotFound(null);
			}
			
			var operationResults = await binacleService.FitBinsAsync(
				presetOption.Bins!,
				request.Items!,
				new FittingParameters
				{
					Algorithm = request.Parameters!.Algorithm!.Value,
					ReportFittedItems =true,
					ReportUnfittedItems = true
				}
			);

			using (var responseActivity = Diagnostics.ActivitySource.StartActivity("Create Response"))
			{
				return Results.Ok(
					FitResponse.Create(
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

