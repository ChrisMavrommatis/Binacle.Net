﻿using FluentAssertions;
using System.Net.Http.Json;
using Xunit;

namespace Binacle.Net.Api.IntegrationTests.v2;

[Collection(BinacleApiCollection.Name)]
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class FitByCustomBehavior
{
	private readonly BinacleApiFactory sut;
	private readonly Api.v2.Requests.CustomFitRequest sampleRequest = new()
	{
		Parameters = new()
		{
			FindSmallestBinOnly = false,
			ReportFittedItems = false,
			ReportUnfittedItems = false
		},
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

	private const string routePath = "/api/v2/fit/by-custom";

	public FitByCustomBehavior(BinacleApiFactory sut)
	{
		this.sut = sut;
	}

	[Fact(DisplayName = $"POST {routePath}. With Valid Request Returns 200 OK")]
	public async Task Post_WithValidRequest_Returns_200Ok()
	{
		var response = await sut.Client.PostAsJsonAsync(routePath, sampleRequest, sut.JsonSerializerOptions);

		response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
	}

	[Fact(DisplayName = $"POST {routePath}. With Zero Dimension On Item Returns 400 BadRequest")]
	public async Task Post_WithZeroDimensionOnItem_Returns_400BadRequest()
	{
		this.sampleRequest.Items!.FirstOrDefault(x => x.ID == "box_2")!.Length = 0;

		var result = await sut.Client.PostAsJsonAsync(routePath, sampleRequest, sut.JsonSerializerOptions);

		Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
	}

	[Fact(DisplayName = $"POST {routePath}. With Zero Dimension On Bin Returns 400 BadRequest")]
	public async Task Post_WithZeroDimensionOnBin_Returns400BadRequest()
	{
		this.sampleRequest.Bins!.FirstOrDefault(x => x.ID == "custom_bin_1")!.Length = 0;

		var result = await sut.Client.PostAsJsonAsync(routePath, sampleRequest, sut.JsonSerializerOptions);

		Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
	}

	[Fact(DisplayName = $"POST {routePath}. With Same Id On Bins Returns 400 BadRequest")]
	public async Task Post_WithSameIdOnBins_Returns400BadRequest()
	{
		foreach (var bin in this.sampleRequest.Bins!)
		{
			bin.ID = "custom_bin_1";
		}

		var result = await sut.Client.PostAsJsonAsync(routePath, sampleRequest, sut.JsonSerializerOptions);

		Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
	}

	[Fact(DisplayName = $"POST {routePath}. With Same Id On Items Returns 400 BadRequest")]
	public async Task Post_WithSameIdOnItems_Returns400BadRequest()
	{
		foreach (var bin in this.sampleRequest.Items!)
		{
			bin.ID = "box_1";
		}

		var result = await sut.Client.PostAsJsonAsync(routePath, sampleRequest, sut.JsonSerializerOptions);

		Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
	}
}
