using Asp.Versioning;
using Binacle.Net.Api.Models;
using Binacle.Net.Api.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.Api.Controllers;

public partial class QueryController
{
	/// <summary>
	/// Lists the presets present in configuration
	/// </summary>
	/// <returns>All of the configured presets wth the associated bins</returns>
	/// <response code="200">Returns the all of the configured presets wth the associated bins</response>
	/// <response code="500">If an unexpected error occurs</response>
	[HttpGet]
	[Route("presets")]
	[Consumes("application/json")]
	[Produces("application/json")]
	[MapToApiVersion("1.0")]
	[ProducesResponseType(typeof(Dictionary<string, List<Bin>>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> PresetsList()
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
