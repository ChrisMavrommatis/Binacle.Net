﻿using Binacle.Net.Api.Configuration.Models;
using Binacle.Net.Api.v1.Requests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Xunit;

namespace Binacle.Net.Api.IntegrationTests;

[Collection(BinacleApiCollection.Name)]
public class QueryByPresetEndpointTests : IClassFixture<BinacleApiFactory>
{
	private readonly BinacleApiFactory apiFactory;
	private readonly IOptions<BinPresetOptions> presetOptions;
	private readonly PresetQueryRequest sampleRequest = new()
	{
		Items = new()
		{
			new (){ ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
			new (){ ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
			new (){ ID = "box_3", Quantity = 1, Length = 12, Width = 10, Height = 15 },
		}
	};

	private const string routePath = "/api/v1/query/by-preset/{preset}";

	public QueryByPresetEndpointTests(BinacleApiFactory apiFactory)
	{
		this.apiFactory = apiFactory;
		this.presetOptions = this.apiFactory.Services.GetRequiredService<IOptions<BinPresetOptions>>();

	}

	[Fact(DisplayName = $"POST {routePath}. With Non Existing Preset Returns 404 NotFound")]
	public async Task Post_WithNonExistingPreset_Returns_404NotFound()
	{
		var urlPath = routePath.Replace("{preset}", "non-existing-preset");

		var response = await this.apiFactory.Client.PostAsJsonAsync(urlPath, this.sampleRequest, this.apiFactory.JsonSerializerOptions);

		response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
	}

	[Fact(DisplayName = $"POST {routePath}. With Existing Preset And Valid Request Returns 200 OK")]
	public async Task Post_WithExistingPresetAndValidRequest_Returns200Ok()
	{
		var urlPath = routePath.Replace("{preset}", this.presetOptions.Value.Presets.Keys.First());

		var response = await this.apiFactory.Client.PostAsJsonAsync(urlPath, this.sampleRequest, this.apiFactory.JsonSerializerOptions);
		
		response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
	}

	[Fact(DisplayName = $"POST {routePath}. With Zero Dimensions Returns 400 BadRequest")]
	public async Task Post_WithZeroDimensions_Returns400BadRequest()
	{
		this.sampleRequest.Items.FirstOrDefault(x => x.ID == "box_2")!.Length = 0;
		var urlPath = routePath.Replace("{preset}", this.presetOptions.Value.Presets.Keys.First());

		var result = await this.apiFactory.Client.PostAsJsonAsync(urlPath, this.sampleRequest, this.apiFactory.JsonSerializerOptions);

		Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
	}
}
