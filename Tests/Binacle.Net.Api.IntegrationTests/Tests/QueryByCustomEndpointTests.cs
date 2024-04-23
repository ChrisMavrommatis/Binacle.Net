using Binacle.Net.Api.v1.Requests;
using FluentAssertions;
using System.Net.Http.Json;
using Xunit;

namespace Binacle.Net.Api.IntegrationTests.Tests;

[Collection(BinacleApiCollection.Name)]
public class QueryByCustomEndpointTests
{
	private readonly BinacleApiFactory apiFactory;
	private readonly CustomQueryRequest sampleRequest = new()
	{
		Bins = new()
		{
			new() { ID = "custom_bin_1", Length = 10, Width = 40, Height = 60 },
			new() { ID = "custom_bin_2", Length = 20, Width = 40, Height = 60 },
			new() { ID = "custom_bin_3", Length = 30, Width = 40, Height = 60 },
		},
		Items = new()
		{
			new (){ ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
			new (){ ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
			new (){ ID = "box_3", Quantity = 1, Length = 12, Width = 10, Height = 15 },
		}
	};

	private const string routePath = "/api/v1/query/by-custom";

	public QueryByCustomEndpointTests(BinacleApiFactory apiFactory)
	{
		this.apiFactory = apiFactory;
	}

	[Fact]
	public async Task Query_WithValidRequest_Returns200Ok()
	{
		var response = await this.apiFactory.Client.PostAsJsonAsync(routePath, this.sampleRequest, this.apiFactory.JsonSerializerOptions);
		
		response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
	}

	[Fact]
	public async Task Query_WithZeroDimensionOnItem_Returns400BadRequest()
	{
		this.sampleRequest.Items.FirstOrDefault(x => x.ID == "box_2")!.Length = 0;

		var result = await this.apiFactory.Client.PostAsJsonAsync(routePath, this.sampleRequest, this.apiFactory.JsonSerializerOptions);

		Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
	}

	[Fact]
	public async Task Query_WithZeroDimensionOnBin_Returns400BadRequest()
	{
		this.sampleRequest.Bins.FirstOrDefault(x => x.ID == "custom_bin_1")!.Length = 0;

		var result = await this.apiFactory.Client.PostAsJsonAsync(routePath, this.sampleRequest, this.apiFactory.JsonSerializerOptions);

		Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
	}
}
