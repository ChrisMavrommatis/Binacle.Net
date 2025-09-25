
using Binacle.Net.IntegrationTests.v3.Abstractions;
using Binacle.Net.Models;
using Binacle.Net.v2.Requests;
using Binacle.Net.v3.Contracts;
using FitRequestParameters = Binacle.Net.v3.Contracts.FitRequestParameters;

namespace Binacle.Net.IntegrationTests.v3;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class FitByCustomBehavior : BehaviourTestsBase
{
	private readonly FitByCustomRequest sampleRequest = new()
	{
		Parameters = new()
		{
			Algorithm = Algorithm.FFD
		},
		Bins = new()
		{
			new() { ID = "custom_bin_1", Length = 10, Width = 40, Height = 60 },
			new() { ID = "custom_bin_2", Length = 20, Width = 40, Height = 60 },
			new() { ID = "custom_bin_3", Length = 30, Width = 40, Height = 60 },
		},
		Items = new()
		{
			new() { ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
			new() { ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
			new() { ID = "box_3", Quantity = 1, Length = 12, Width = 10, Height = 15 },
		}
	};


	private const string routePath = "/api/v3/fit/by-custom";

	public FitByCustomBehavior(BinacleApi sut) : base(sut)
	{
	}

	#region Response Statuses

	[Fact(DisplayName = $"POST {routePath}. With Valid Request, Returns 200 OK")]
	public Task Post_WithValidRequest_Returns_200Ok()
		=> base.Request_Returns_200Ok(routePath, this.sampleRequest);

	[Fact(DisplayName = $"POST {routePath}. With Zero Dimension On Item, Returns 422 UnprocessableContent")]
	public async Task Post_WithZeroDimensionOnItem_Returns_422UnprocessableContent()
	{
		this.sampleRequest.Items!.FirstOrDefault(x => x.ID == "box_2")!.Length = 0;
		await base.Request_Returns_422UnprocessableContent(routePath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Zero Dimension On Bin, Returns 422 UnprocessableContent")]
	public async Task Post_WithZeroDimensionOnBin_422UnprocessableContent()
	{
		this.sampleRequest.Bins!.FirstOrDefault(x => x.ID == "custom_bin_1")!.Length = 0;
		await base.Request_Returns_422UnprocessableContent(routePath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Same Id On Bins, Returns 400 BadRequest")]
	public async Task Post_WithSameIdOnBins_Returns400BadRequest()
	{
		foreach (var bin in this.sampleRequest.Bins!)
		{
			bin.ID = "custom_bin_1";
		}

		await base.Request_Returns_422UnprocessableContent(routePath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Same Id On Items, Returns 422 UnprocessableContent")]
	public async Task Post_WithSameIdOnItems_Returns422UnprocessableContent()
	{
		foreach (var bin in this.sampleRequest.Items!)
		{
			bin.ID = "box_1";
		}
		await base.Request_Returns_422UnprocessableContent(routePath, this.sampleRequest);
	}

	#endregion Response Statuses

	#region Response Data
	
	[Fact(DisplayName = $"POST {routePath}. With Large Volume, Returns With Early Fail Total Volume Exceeded")]
	public async Task Post_WithLargeVolume_ReturnsWithEarlyFail_TotalVolumeExceeded()
	{
		var request = this.CreateSpecialRequest();
		request.Items!.FirstOrDefault(x => x.ID == "special_box_1")!.Quantity = 3;

		await base.FitRequest_ValidateBasedOnParameters(
			routePath,
			request,
			result =>
			{
				result.Data.ShouldHaveCount(request.Bins!.Count);
				foreach (var binFitResult in result.Data)
				{
					binFitResult.Result.ShouldBe(BinFitResultStatus.EarlyFail_TotalVolumeExceeded);
				}
			}
		);
	}

	[Fact(DisplayName = $"POST {routePath}. With Large Volume, Returns With Early Fail Item Dimension Exceeded")]
	public async Task Post_WithLargeDimension_ReturnsWithEarlyFail_ItemDimensionExceeded()
	{
		var request = this.CreateSpecialRequest();
		var specialBox = request.Items!.FirstOrDefault(x => x.ID == "special_box_1")!;
		specialBox.Length = 61;
		specialBox.Width = 5;
		specialBox.Height = 5;
		
		await base.FitRequest_ValidateBasedOnParameters(
			routePath,
			request,
			result =>
			{
				result.Data.ShouldHaveCount(request.Bins!.Count);
				foreach (var binFitResult in result.Data)
				{
					binFitResult.Result.ShouldBe(BinFitResultStatus.EarlyFail_ItemDimensionExceeded);
				}
			}
		);
	}

	private FitByCustomRequest CreateSpecialRequest(
		Action<FitRequestParameters>? modifyParameters = null
	)
	{
		var request = new FitByCustomRequest
		{
			Parameters = new()
			{
				Algorithm = Algorithm.FFD
			},
			Bins = new()
			{
				new() { ID = "special_bin_1", Length = 10, Width = 40, Height = 60 },
				new() { ID = "special_bin_2", Length = 11, Width = 40, Height = 60 },
				new() { ID = "special_bin_3", Length = 12, Width = 40, Height = 60 },
			},
			Items = new()
			{
				new() { ID = "special_box_1", Quantity = 1, Length = 8, Width = 40, Height = 60 },
				new() { ID = "box_1", Quantity = 1, Length = 5, Width = 5, Height = 5 },
			}
		};
		modifyParameters?.Invoke(request.Parameters!);
		return request;
	}
	#endregion Response Data
}
