using System.Collections.Immutable;
using System.Net.Mime;
using Binacle.Net.Configuration;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.v3.Contracts;
using Microsoft.Extensions.Options;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v3.Endpoints.Presets;

internal class List : IGroupedEndpoint<ApiV3EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapGet("presets", HandleAsync)
			.WithOperationId("listPresets")
			.WithTags("Presets")
			.WithSummary("List presets")
			.WithDescription("Lists the presets present in configuration.")
			.Produces<PresetListResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
			.ResponseExample<PresetListResponseExample>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
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
			var emptyResponse = PresetListResponse.Create(
				ImmutableDictionary<string, List<Bin>>.Empty
			);

			return Results.Ok(emptyResponse);
		}

		var presets = presetOptions.Value.Presets
			.ToDictionary(
				x => x.Key,
				x => x.Value.Bins
					.Select(bin => new Bin()
					{
						ID = bin.ID,
						Length = bin.Length,
						Height = bin.Height,
						Width = bin.Width
					}).ToList()
			);

		var response = PresetListResponse.Create(presets);

		return Results.Ok(response);
	}
}
