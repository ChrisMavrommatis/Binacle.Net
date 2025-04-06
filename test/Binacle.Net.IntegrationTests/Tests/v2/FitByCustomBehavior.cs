using Binacle.Net.IntegrationTests.v2.Abstractions;
using Binacle.Net.v2.Models;
using Binacle.Net.v2.Requests;

namespace Binacle.Net.IntegrationTests.v2;

[Collection(BinacleApiCollection.Name)]
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class FitByCustomBehavior : BehaviourTestsBase
{
	private readonly CustomFitRequest sampleRequest = new()
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
			new() { ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
			new() { ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
			new() { ID = "box_3", Quantity = 1, Length = 12, Width = 10, Height = 15 },
		}
	};


	private const string routePath = "/api/v2/fit/by-custom";

	public FitByCustomBehavior(BinacleApiFactory sut) : base(sut)
	{
	}

	#region Response Statuses

	[Fact(DisplayName = $"POST {routePath}. With Valid Request, Returns 200 OK")]
	public Task Post_WithValidRequest_Returns_200Ok()
		=> base.Request_Returns_200Ok(routePath, this.sampleRequest);

	[Fact(DisplayName = $"POST {routePath}. With Zero Dimension On Item, Returns 400 BadRequest")]
	public async Task Post_WithZeroDimensionOnItem_Returns_400BadRequest()
	{
		this.sampleRequest.Items!.FirstOrDefault(x => x.ID == "box_2")!.Length = 0;
		await base.Request_Returns_400BadRequest(routePath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Zero Dimension On Bin, Returns 400 BadRequest")]
	public async Task Post_WithZeroDimensionOnBin_Returns400BadRequest()
	{
		this.sampleRequest.Bins!.FirstOrDefault(x => x.ID == "custom_bin_1")!.Length = 0;
		await base.Request_Returns_400BadRequest(routePath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Same Id On Bins, Returns 400 BadRequest")]
	public async Task Post_WithSameIdOnBins_Returns400BadRequest()
	{
		foreach (var bin in this.sampleRequest.Bins!)
		{
			bin.ID = "custom_bin_1";
		}

		await base.Request_Returns_400BadRequest(routePath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Same Id On Items, Returns 400 BadRequest")]
	public async Task Post_WithSameIdOnItems_Returns400BadRequest()
	{
		foreach (var bin in this.sampleRequest.Items!)
		{
			bin.ID = "box_1";
		}
		await base.Request_Returns_400BadRequest(routePath, this.sampleRequest);
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

	[Fact(DisplayName = $"POST {routePath}. With Report Fitted Items, Returns With Fitted Items")]
	public async Task Post_WithReportFittedItems_ReturnsFittedItems()
	{
		var request = this.CreateSpecialRequest(parameters => parameters.ReportFittedItems = true);

		await base.FitRequest_ValidateBasedOnParameters(
			routePath,
			request,
			result =>
			{
				result.Data.ShouldHaveCount(request.Bins!.Count);
			}
		);
	}

	[Fact(DisplayName = $"POST {routePath}. With Report Unfitted Items, Returns With Unfitted Items")]
	public async Task Post_WithReportUnfittedItems_ReturnsUnfittedItems()
	{
		var request = this.CreateSpecialRequest(parameters => parameters.ReportUnfittedItems = true);

		await base.FitRequest_ValidateBasedOnParameters(
			routePath,
			request,
			result =>
			{
				result.Data.ShouldHaveCount(request.Bins!.Count);
			}
		);
	}

	[Fact(DisplayName = $"POST {routePath}. With Find Smallest Bin Only, Returns With Smallest Bin Only")]
	public async Task Post_WithFindSmallestBinOnly_ReturnsWithSmallestBinOnly()
	{
		this.sampleRequest.Parameters!.FindSmallestBinOnly = true;
		var expectedBin = this.sampleRequest.Bins!
			.OrderBy(x => x.Length * x.Width * x.Height)
			.FirstOrDefault()!;

		await base.FitRequest_ValidateBasedOnParameters(
			routePath,
			this.sampleRequest,
			result =>
			{
				foreach (var binFitResult in result.Data)
				{
					binFitResult.Bin.ID.ShouldBe(expectedBin.ID);
				}
			}
		);
	}

	private CustomFitRequest CreateSpecialRequest(
		Action<FitRequestParameters>? modifyParameters = null
	)
	{
		var request = new CustomFitRequest
		{
			Parameters = new()
			{
				FindSmallestBinOnly = false,
				ReportFittedItems = false,
				ReportUnfittedItems = false
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
