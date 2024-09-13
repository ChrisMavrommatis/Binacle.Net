using Asp.Versioning;
using Binacle.Net.Api.Configuration.Models;
using ChrisMavrommatis.Endpoints;
using ChrisMavrommatis.SwaggerExamples.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace Binacle.Net.Api.v1.Endpoints.Presets;

/// <summary>
/// List Presets Endpoint
/// </summary>
[ApiVersion(v1.ApiVersion.Number, Deprecated = v1.ApiVersion.IsDeprecated)]
[Route("api/v{version:apiVersion}/[namespace]")]
public class List : EndpointWithoutRequest
{
	private readonly IOptions<BinPresetOptions> presetOptions;
	private readonly ILogger<List> logger;

	/// <summary>
	/// List Presets Endpoint
	/// </summary>
	/// <param name="presetOptions"></param>
	/// <param name="logger"></param>
	public List(
		IOptions<BinPresetOptions> presetOptions,
		ILogger<List> logger
		)
	{
		this.presetOptions = presetOptions;
		this.logger = logger;
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
	[MapToApiVersion(v1.ApiVersion.Number)]

	[HttpGet]
	[ProducesResponseType(typeof(v1.Responses.PresetListResponse), StatusCodes.Status200OK)]
	[SwaggerResponseExample(typeof(v1.Responses.PresetListResponse), typeof(v1.Responses.Examples.PresetListResponseExample), StatusCodes.Status200OK)]

	[ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]

	[ProducesResponseType(typeof(v1.Responses.ErrorResponse), StatusCodes.Status500InternalServerError)]
	[SwaggerResponseExample(typeof(v1.Responses.ErrorResponse), typeof(v1.Responses.Examples.ServerErrorResponseExample), StatusCodes.Status500InternalServerError)]

	#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public override async Task<IActionResult> HandleAsync(CancellationToken cancellationToken = default)
	#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		try
		{
			if (this.presetOptions?.Value?.Presets is null || this.presetOptions.Value.Presets.Count <= 0)
			{
				return this.NotFound(null);
			}

			var presets = this.presetOptions.Value.Presets
			.ToDictionary(
				x => x.Key,
				x => x.Value.Bins
					.Select(bin => new v1.Models.Bin()
					{
						ID = bin.ID,
						Length = bin.Length,
						Height = bin.Height,
						Width = bin.Width
					}).ToList()
			);

			var response = v1.Responses.PresetListResponse.Create(presets);

			return this.Ok(response);
		}
		catch (Exception ex)
		{
			this.logger.LogError(ex, "An exception occurred in {endpoint} endpoint", "List Presets");
			return this.InternalServerError(
				v1.Responses.ErrorResponse.Create(Constants.Errors.Categories.ServerError)
				.AddExceptionError(ex)
				);
		}
	}
}
