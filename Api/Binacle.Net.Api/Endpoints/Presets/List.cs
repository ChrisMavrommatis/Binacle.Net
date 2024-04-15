using Asp.Versioning;
using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.Api.Models;
using Binacle.Net.Api.Models.Responses;
using Binacle.Net.Api.Models.Responses.Examples;
using ChrisMavrommatis.Endpoints;
using ChrisMavrommatis.SwaggerExamples.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Binacle.Net.Api.Endpoints.Presets;

[Route("api/[namespace]")]
public class List : EndpointWithoutRequest
{
	private readonly IOptions<BinPresetOptions> presetOptions;

	public List(IOptions<BinPresetOptions> presetOptions)
	{
		this.presetOptions = presetOptions;
	}

	/// <summary>
	/// Lists the presets present in configuration.
	/// </summary>
	/// <response code="200"> <b>OK</b>
	/// <br />
	/// <p>
	///		Returns the all of the configured presets wth the associated bins.
	/// </p>
	/// </response>
	/// <response code="404"> <b>Not Found</b>
	/// <br />
	/// <p>
	///		If no presets are configured.
	/// </p>
	/// </response>
	/// <response code="500"> <b>Internal Server Error</b>
	/// <br />
	/// <p>
	///		If an unexpected error occurs.
	/// </p>
	/// <p>
	///		Exception details will only be shown when in a development environment.
	/// </p>
	/// </response>
	[Consumes("application/json")]
	[Produces("application/json")]
	[HttpGet]
	[ProducesResponseType(typeof(PresetListResponse), StatusCodes.Status200OK)]
	[SwaggerResponseExample(typeof(PresetListResponse), typeof(PresetListResponseExample), StatusCodes.Status200OK)]

	[ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]

	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
	[SwaggerResponseExample(typeof(ErrorResponse), typeof(ServerErrorResponseExample), StatusCodes.Status500InternalServerError)]
	public override async Task<IActionResult> HandleAsync(CancellationToken cancellationToken = default)
	{
		try
		{
			if (this.presetOptions?.Value?.Presets is null || !this.presetOptions.Value.Presets.Any())
			{
				return this.NotFound(null);
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
				ErrorResponse.Create(Constants.Errors.Categories.ServerError)
				.AddExceptionError(ex)
				);
		}
	}
}
