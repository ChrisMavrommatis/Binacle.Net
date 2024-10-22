using Binacle.Net.Api.Configuration.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Xunit;

namespace Binacle.Net.Api.IntegrationTests.v1;


[Collection(BinacleApiCollection.Name)]
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class QueryByPresetBehavior
{
	private readonly BinacleApiFactory sut;
	private readonly IOptions<BinPresetOptions> presetOptions;
	private readonly Api.v1.Requests.PresetQueryRequest sampleRequest = new()
	{
		Items = new()
		{
			new (){ ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
			new (){ ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
			new (){ ID = "box_3", Quantity = 1, Length = 12, Width = 10, Height = 15 },
		}
	};

	private const string routePath = "/api/v1/query/by-preset/{preset}";

	public QueryByPresetBehavior(BinacleApiFactory sut)
	{
		this.sut = sut;
		presetOptions = this.sut.Services.GetRequiredService<IOptions<BinPresetOptions>>();

	}

	[Fact(DisplayName = $"POST {routePath}. With Non Existing Preset Returns 404 NotFound")]
	public async Task Post_WithNonExistingPreset_Returns_404NotFound()
	{
		var urlPath = routePath.Replace("{preset}", "non-existing-preset");

		var response = await sut.Client.PostAsJsonAsync(urlPath, this.sampleRequest!, sut.JsonSerializerOptions);

		response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
	}

	[Fact(DisplayName = $"POST {routePath}. With Existing Preset And Valid Request Returns 200 OK")]
	public async Task Post_WithExistingPresetAndValidRequest_Returns200Ok()
	{
		var urlPath = routePath.Replace("{preset}", presetOptions.Value.Presets.Keys.First());

		var response = await sut.Client.PostAsJsonAsync(urlPath, this.sampleRequest!, sut.JsonSerializerOptions);

		response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
	}

	[Fact(DisplayName = $"POST {routePath}. With Zero Dimensions Returns 400 BadRequest")]
	public async Task Post_WithZeroDimensions_Returns400BadRequest()
	{
		this.sampleRequest!.Items!.FirstOrDefault(x => x.ID == "box_2")!.Length = 0;
		var urlPath = routePath.Replace("{preset}", presetOptions.Value.Presets.Keys.First());

		var result = await sut.Client.PostAsJsonAsync(urlPath, sampleRequest, sut.JsonSerializerOptions);

		Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
	}

	[Fact(DisplayName = $"POST {routePath}. With Same Id On Items Returns 400 BadRequest")]
	public async Task Post_WithSameIdOnItems_Returns400BadRequest()
	{
		foreach (var bin in this.sampleRequest!.Items!)
		{
			bin.ID = "box_1";
		}

		var result = await sut.Client.PostAsJsonAsync(routePath, this.sampleRequest!, sut.JsonSerializerOptions);

		Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
	}
}
