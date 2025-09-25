using Binacle.Net.Configuration.Models;
using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.v2.Models;
using Binacle.Net.v2.Responses;
using Binacle.Net.v2.Responses.Examples;
using Microsoft.Extensions.Options;
using OpenApiExamples.ExtensionMethods;

namespace Binacle.Net.v2.Endpoints.Presets;

internal class List : IGroupedEndpoint<ApiV2EndpointGroup>
{
	public void DefineEndpoint(RouteGroupBuilder group)
	{
		group.MapGet("presets", HandleAsync)
			.WithTags("Presets")
			.WithSummary("List Presets")
			.WithDescription("Lists the presets present in configuration.")
			.Produces<PresetListResponse>(StatusCodes.Status200OK, "application/json")
			.ResponseExample<PresetListResponseExample>(StatusCodes.Status200OK, "application/json")
			.ResponseDescription(StatusCodes.Status200OK, "Returns all of the configured presets wth the associated bins.")
			.Produces(StatusCodes.Status404NotFound)
			.ResponseDescription(StatusCodes.Status404NotFound, "If no presets are configured.");
	}
	
	#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	internal async Task<IResult> HandleAsync(
		IOptions<BinPresetOptions> presetOptions,
		ILogger<List> logger,
		CancellationToken cancellationToken = default
	)
	{
		try
		{
			if (presetOptions?.Value?.Presets is null || presetOptions.Value.Presets.Count <= 0)
			{
				return Results.NotFound(null);
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
		catch (Exception ex)
		{
			logger.LogError(ex, "An exception occurred in {endpoint} endpoint", "List Presets");
			return Results.InternalServerError(
				Response.ExceptionError(ex, ErrorCategory.ServerError)
			);
		}
	}
}
