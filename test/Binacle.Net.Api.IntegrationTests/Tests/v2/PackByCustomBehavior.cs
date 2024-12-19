using FluentAssertions;
using System.Net.Http.Json;
using Xunit;

namespace Binacle.Net.Api.IntegrationTests.v2;

[Collection(BinacleApiCollection.Name)]
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class PackByCustomBehavior : Abstractions.BehaviourTestsBase
{
	private readonly Api.v2.Requests.CustomPackRequest sampleRequest = new()
	{
		Parameters = new()
		{
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false,
			OptInToEarlyFails = false,
			StopAtSmallestBin = false,
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

	
	private const string routePath = "/api/v2/pack/by-custom";

	public PackByCustomBehavior(BinacleApiFactory sut) : base(sut)
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
	
	[Fact(DisplayName = $"POST {routePath}. With Default Parameters, Reports All Items")]
	public async Task Post_WithDefaultParameters_ReportsAllItems()
	{
		var request = this.CreateSpecialRequest();

		await base.PackRequest_ValidateBasedOnParameters(
			routePath,
			request,
			result =>
			{
				result.Data.Should().HaveCount(request.Bins!.Count);
			}
		);
	}
	
	[Fact(DisplayName = $"POST {routePath}. With Report Packed Items Only When Fully Packed, Does Not Report Them On Partial Pack")]
	public async Task Post_WithReportPackedItemsOnlyWhenFullyPacked_DoesNotReportThemOnPartialPack()
	{
		var request = this.CreateSpecialRequest(parameters => parameters.ReportPackedItemsOnlyWhenFullyPacked = true);

		await base.PackRequest_ValidateBasedOnParameters(
			routePath,
			request,
			result =>
			{
				result.Data.Should().HaveCount(request.Bins!.Count);
			}
		);
	}
	
	[Fact(DisplayName = $"POST {routePath}. With Never Report Unpacked Items, Does Not Report Them On Partial Pack")]
	public async Task Post_WithNeverReportUnpackedItems_DoesNotReportThemOnPartialPack()
	{
		var request = this.CreateSpecialRequest(parameters => parameters.NeverReportUnpackedItems = true);

		await base.PackRequest_ValidateBasedOnParameters(
			routePath,
			request,
			result =>
			{
				result.Data.Should().HaveCount(request.Bins!.Count);
			}
		);
	}

	[Fact(DisplayName = $"POST {routePath}. With Opt In To Early Fails And Large Volume, Returns Early Fail Container Volume Exceeded")]
	public async Task Post_WithEarlyFailOptInAndLargeVolume_ReturnsWithEarlyFail_ContainerVolumeExceeded()
	{
		var request = this.CreateSpecialRequest(parameters => parameters.OptInToEarlyFails = true);
		request.Items!.FirstOrDefault(x => x.ID == "special_box_1")!.Quantity = 3;

		await base.PackRequest_ValidateBasedOnParameters(
			routePath,
			request,
			result =>
			{
				result.Data.Should().HaveCount(request.Bins!.Count);
				foreach (var binPackResult in result.Data)
				{
					binPackResult.Result.Should().Be(Api.v2.Models.BinPackResultStatus.EarlyFail_ContainerVolumeExceeded);
				}
			}
		);
	}
	
	[Fact(DisplayName = $"POST {routePath}. With Early Fail Opt In And Large Dimension, Returns Early Fail Container Dimension Exceeded")]
	public async Task Post_WithEarlyFailOptInAndLargeDimension_ReturnsWithEarlyFail_ContainerDimensionExceeded()
	{
		var request = this.CreateSpecialRequest(parameters => parameters.OptInToEarlyFails = true);
		var specialBox = request.Items!.FirstOrDefault(x => x.ID == "special_box_1")!;
		specialBox.Length = 61;
		specialBox.Width = 5;
		specialBox.Height = 5;

		await base.PackRequest_ValidateBasedOnParameters(
			routePath,
			request,
			result =>
			{
				result.Data.Should().HaveCount(request.Bins!.Count);
				foreach (var binPackResult in result.Data)
				{
					binPackResult.Result.Should().Be(Api.v2.Models.BinPackResultStatus.EarlyFail_ContainerDimensionExceeded);

				}
			}
		);
	}
	
	[Fact(DisplayName = $"POST {routePath}. With Stop At Smallest Bin, Returns With Smallest Bin Only")]
	public async Task Post_WithStopAtSmallestBin_ReturnsWithSmallestBinOnly()
	{
		this.sampleRequest.Parameters!.StopAtSmallestBin = true;
		var expectedBin = this.sampleRequest.Bins!
			.OrderBy(x => x.Length * x.Width * x.Height)
			.FirstOrDefault()!;
		
		await base.PackRequest_ValidateBasedOnParameters(
			routePath,
			this.sampleRequest,
			result =>
			{
				foreach (var binPackResult in result.Data)
				{
					binPackResult.Bin.ID.Should().Be(expectedBin.ID);
				};
			}
		);
	}
	
	private Api.v2.Requests.CustomPackRequest CreateSpecialRequest(
		Action<Api.v2.Requests.PackRequestParameters> modifyParameters = null
	)
	{
		var request = new Api.v2.Requests.CustomPackRequest
		{
			Parameters = new()
			{
				NeverReportUnpackedItems = false,
				ReportPackedItemsOnlyWhenFullyPacked = false,
				OptInToEarlyFails = false,
				StopAtSmallestBin = false,
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
