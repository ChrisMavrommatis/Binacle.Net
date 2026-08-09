using Binacle.Net.Configuration;
using System.ComponentModel;
using System.Net.Mime;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.v3.Contracts;
using Binacle.Net.Services;
using Binacle.Net.v3.ExtensionMethods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v3.Endpoints.Fit;

internal class ByPreset : IGroupedEndpoint<ApiV3EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapPost("fit/by-preset/{preset}", HandleAsync)
			.WithOperationId("fitByPreset")
			.WithTags("Fit")
			.WithSummary("Fit by preset")
			.WithDescription("Perform a bin fit function using a specified bin preset.")
			
			.Accepts<FitByPresetRequest>(MediaTypeNames.Application.Json)
			.RequestExample<FitByPresetRequestExample>(MediaTypeNames.Application.Json)
			
			.Produces<FitResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
			.ResponseExamples<FitByPresetResponseExamples>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
			.ResponseDescription(StatusCodes.Status200OK, ResponseDescription.ForFitResponse200Ok)
			
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ResponseDescription(StatusCodes.Status400BadRequest, ResponseDescription.For400BadRequest)
			.ResponseExamples<Status400ResponseExamples>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)
			
			.Produces(StatusCodes.Status404NotFound)
			.ResponseDescription(StatusCodes.Status404NotFound, ResponseDescription.ForPreset404NotFound)
			
			.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
			.ResponseDescription(
				StatusCodes.Status422UnprocessableEntity,
				ResponseDescription.For400BadRequest
			)
			.ResponseExamples<FitByPresetValidationProblemExamples>(
				StatusCodes.Status422UnprocessableEntity,
				MediaTypeNames.Application.ProblemJson
			)
			.RequireRateLimiting("ApiUsage")
			.RequireCors(CorsPolicy.CoreApi);
	}

	internal static async Task<IResult> HandleAsync(
		[FromRoute][Description(SchemaDescriptions.PresetParam)] string preset,
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
			
			var operationResults = await binacleService.MultipleBinsAsync(
				request.Parameters.Algorithm.ToLibAlgorithm(),
				presetOption.Bins,
				request.Items,
				request.Parameters,
				cancellationToken
			);

			using (var responseActivity = Diagnostics.ActivitySource.StartActivity("Create Response"))
			{
				return Results.Ok(
					FitResponse.Create(
						presetOption.Bins,
						request.Items,
						request.Parameters,
						operationResults
					)
				);
			}
		});
	}
}

