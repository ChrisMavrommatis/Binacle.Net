using System.Collections.Immutable;
using Binacle.Net.Configuration;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.v4.Contracts.Presets;
using Binacle.Net.v4.Contracts;
using Microsoft.Extensions.Options;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v4.Endpoints.Presets;

internal class List : IGroupedEndpoint<ApiV4EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapGet("presets", HandleAsync)
			.WithTags("Presets")
			.WithSummary("List Presets")
			.WithDescription("Lists the presets present in configuration.")
			.Produces<PresetListResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseExample<PresetListResponseExample>(StatusCodes.Status200OK, "application/json")
			.ResponseDescription(StatusCodes.Status200OK,
				"Returns all of the configured presets wth the associated bins."
			).RequireCors(CorsPolicy.CoreApi);
	}
	
	#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	internal async Task<IResult> HandleAsync(
		IOptions<BinPresetOptions> presetOptions,
		ILogger<List> logger,
		CancellationToken cancellationToken = default
	)
	{
		if (presetOptions?.Value?.Presets is null || presetOptions.Value.Presets.Count <= 0)
		{
			var emptyResponse = PresetListResponse.From(
				ImmutableDictionary<string, List<Bin>>.Empty
			);

			return Results.Ok(emptyResponse);
		}

		var response = PresetListResponse.From(presetOptions.Value.Presets);

		return Results.Ok(response);
	}
}
