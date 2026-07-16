using Binacle.Net.Configuration;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.v4.Contracts.Presets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v4.Endpoints.Presets;

internal class Get : IGroupedEndpoint<ApiV4EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapGet("presets/{preset}", HandleAsync)
			.WithTags("Presets")
			.WithSummary("Get a preset")
			.WithDescription("Gets the bins configured for a single preset.")

			.Produces<PresetResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseExample<PresetResponseExample>(StatusCodes.Status200OK, "application/json")
			.ResponseDescription(StatusCodes.Status200OK, "Returns the preset with its associated bins.")

			.Produces(StatusCodes.Status404NotFound)
			.ResponseDescription(StatusCodes.Status404NotFound, "If the preset does not exist.")

			.RequireCors(CorsPolicy.CoreApi);
	}

	#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	internal async Task<IResult> HandleAsync(
		[FromRoute] string preset,
		IOptions<BinPresetOptions> presetOptions,
		ILogger<Get> logger,
		CancellationToken cancellationToken = default
	)
	{
		if (!presetOptions.Value.TryGetPreset(preset, out var presetOption))
		{
			return Results.NotFound(null);
		}

		return Results.Ok(PresetResponse.From(preset, presetOption));
	}
}
