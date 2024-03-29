using Asp.Versioning;
using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.Api.Models;
using Binacle.Net.Api.Models.Responses;
using ChrisMavrommatis.Endpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Binacle.Net.Api.Endpoints.Presets;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[namespace]")]
public class List : EndpointWithoutRequest
{
	private readonly IOptions<BinPresetOptions> presetOptions;

	public List(IOptions<BinPresetOptions> presetOptions)
	{
		this.presetOptions = presetOptions;
	}

	/// <summary>
	/// Lists the presets present in configuration
	/// </summary>
	/// <returns>All of the configured presets wth the associated bins</returns>
	/// <response code="200">Returns the all of the configured presets wth the associated bins</response>
	/// <response code="404">If no presets are configured</response>
	/// <response code="500">If an unexpected error occurs</response>
	[Consumes("application/json")]
	[Produces("application/json")]
	[MapToApiVersion("1.0")]
	[HttpGet]
	[ProducesResponseType(typeof(PresetListResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
	public override async Task<IActionResult> HandleAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			if (this.presetOptions?.Value?.Presets is null || !this.presetOptions.Value.Presets.Any())
			{
				return this.NotFound(
					ErrorResponse.Create("No presets found")
					);
			}

			var presets = this.presetOptions.Value.Presets
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

			return this.Ok(response);
		}
		catch (Exception ex)
		{
			return this.InternalServerError(
				ErrorResponse.Create("An internal server error occurred while processing the request.")
				.AddExceptionError(ex)
				);
		}
	}
}
