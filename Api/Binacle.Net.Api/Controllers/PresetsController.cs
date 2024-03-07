using Asp.Versioning;
using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.Api.Models;
using Binacle.Net.Api.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Binacle.Net.Api.Controllers;

[ApiVersion("1.0")]
public class PresetsController : VersionedApiControllerBase
{
	private readonly IOptions<BinPresetOptions> presetOptions;
	public PresetsController(
		IOptions<BinPresetOptions> presetOptions
	  )
	{
		this.presetOptions = presetOptions;
	}

	/// <summary>
	/// Lists the presets present in configuration
	/// </summary>
	/// <returns>All of the configured presets wth the associated bins</returns>
	/// <response code="200">Returns the all of the configured presets wth the associated bins</response>
	/// <response code="500">If an unexpected error occurs</response>
	[HttpGet]
	[Consumes("application/json")]
	[Produces("application/json")]
	[MapToApiVersion("1.0")]
	[ProducesResponseType(typeof(Dictionary<string, List<Bin>>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> Index()
	{
		try
		{
			var presetResponse = this.presetOptions.Value.Presets
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

			return this.Ok(presetResponse);
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
