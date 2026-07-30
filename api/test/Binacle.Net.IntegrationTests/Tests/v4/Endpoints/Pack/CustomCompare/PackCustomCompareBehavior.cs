using Binacle.Net.IntegrationTests.v4.Abstractions;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Pack;

namespace Binacle.Net.IntegrationTests.v4.Endpoints.Pack.CustomCompare;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class PackCustomCompareBehavior : BehaviourTestsBase
{
	private const string routePath = "/api/v4/pack/compare-bins";

	private readonly PackCustomCompareRequest sampleRequest = new()
	{
		Parameters = new() { Algorithm = Algorithm.Best },
		Bins =
		[
			new() { ID = "bin_small", Length = 10, Width = 40, Height = 60 },
			new() { ID = "bin_medium", Length = 20, Width = 40, Height = 60 },
			new() { ID = "bin_large", Length = 30, Width = 40, Height = 60 },
		],
		Items =
		[
			new() { ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
			new() { ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
			new() { ID = "box_3", Quantity = 1, Length = 12, Width = 10, Height = 15 },
		]
	};

	public PackCustomCompareBehavior(BinacleApi sut) : base(sut)
	{
	}

	#region Response Statuses

	[Fact(DisplayName = $"POST {routePath}. With Valid Request, Returns 200 OK")]
	public Task Post_WithValidRequest_Returns_200Ok()
		=> base.Request_Returns_200Ok(routePath, this.sampleRequest);

	[Fact(DisplayName = $"POST {routePath}. Without Algorithm, Returns 422 UnprocessableContent")]
	public async Task Post_WithoutAlgorithm_Returns_422UnprocessableContent()
	{
		this.sampleRequest.Parameters!.Algorithm = null;
		await base.Request_Returns_422UnprocessableContent(routePath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. Without Items, Returns 422 UnprocessableContent")]
	public async Task Post_WithoutItems_Returns_422UnprocessableContent()
	{
		this.sampleRequest.Items = [];
		await base.Request_Returns_422UnprocessableContent(routePath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. Without Bins, Returns 422 UnprocessableContent")]
	public async Task Post_WithoutBins_Returns_422UnprocessableContent()
	{
		this.sampleRequest.Bins = [];
		await base.Request_Returns_422UnprocessableContent(routePath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Zero Dimension On Bin, Returns 422 UnprocessableContent")]
	public async Task Post_WithZeroDimensionOnBin_Returns_422UnprocessableContent()
	{
		this.sampleRequest.Bins!.First(x => x.ID == "bin_small").Length = 0;
		await base.Request_Returns_422UnprocessableContent(routePath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Excessive Item Dimension, Returns 422 UnprocessableContent")]
	public async Task Post_WithExcessiveItemDimension_Returns_422UnprocessableContent()
	{
		this.sampleRequest.Items!.First().Length = 65536;
		await base.Request_Returns_422UnprocessableContent(routePath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Duplicate Bin IDs, Returns 422 UnprocessableContent")]
	public async Task Post_WithDuplicateBinIds_Returns_422UnprocessableContent()
	{
		foreach (var bin in this.sampleRequest.Bins!)
			bin.ID = "bin_small";
		await base.Request_Returns_422UnprocessableContent(routePath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Duplicate Item IDs, Returns 422 UnprocessableContent")]
	public async Task Post_WithDuplicateItemIds_Returns_422UnprocessableContent()
	{
		foreach (var item in this.sampleRequest.Items!)
			item.ID = "box_1";
		await base.Request_Returns_422UnprocessableContent(routePath, this.sampleRequest);
	}

	#endregion

	#region Response Data

	[Fact(DisplayName = $"POST {routePath}. Returns One Result Per Requested Bin")]
	public Task Post_ReturnsOneResultPerBin()
		=> base.PackCompareRequest_Validate(routePath, this.sampleRequest,
			result => result.Results.Count.ShouldBe(this.sampleRequest.Bins!.Count));

	[Fact(DisplayName = $"POST {routePath}. Returns Results In The Order The Bins Were Sent")]
	public Task Post_ReturnsResultsInRequestOrder()
		=> base.PackCompareRequest_Validate(routePath, this.sampleRequest, result =>
			result.Results.Select(x => x.Bin.ID)
				.ShouldBe(this.sampleRequest.Bins!.Select(x => x.ID)));

	[Fact(DisplayName = $"POST {routePath}. With Algorithm FFD, Returns AlgorithmUsed FFD For Every Bin")]
	public async Task Post_WithAlgorithmFFD_ReturnsAlgorithmUsedFFD()
	{
		var request = new PackCustomCompareRequest
		{
			Parameters = new() { Algorithm = Algorithm.FFD },
			Bins = this.sampleRequest.Bins,
			Items = this.sampleRequest.Items
		};
		await base.PackCompareRequest_Validate(routePath, request, result =>
		{
			foreach (var binResult in result.Results)
				binResult.AlgorithmUsed.ShouldBe("FFD");
		});
	}

	[Fact(DisplayName = $"POST {routePath}. With A Bin Too Small, Returns That Bin's Own Failure")]
	public async Task Post_WithOneBinTooSmall_ReturnsPerBinResults()
	{
		var request = new PackCustomCompareRequest
		{
			Parameters = new() { Algorithm = Algorithm.FFD },
			Bins =
			[
				new() { ID = "too_small", Length = 1, Width = 1, Height = 1 },
				new() { ID = "big_enough", Length = 20, Width = 20, Height = 20 },
			],
			Items = [new() { ID = "medium_box", Quantity = 1, Length = 11, Width = 11, Height = 11 }]
		};
		await base.PackCompareRequest_Validate(routePath, request, result =>
		{
			result.Results.Count.ShouldBe(2);
			result.Results.First(x => x.Bin.ID == "too_small").Status.ShouldBe(BinPackResultStatus.NotPacked);
			result.Results.First(x => x.Bin.ID == "big_enough").Status.ShouldBe(BinPackResultStatus.FullyPacked);
		});
	}

	[Fact(DisplayName = $"POST {routePath}. Without IncludeViPaqData, Returns No ViPaqData")]
	public Task Post_WithoutIncludeViPaqData_Returns_NoViPaqData()
		=> base.PackCompareRequest_Validate(routePath, this.sampleRequest, result =>
		{
			foreach (var binResult in result.Results)
				binResult.ViPaqData.ShouldBeNull();
		});

	[Fact(DisplayName = $"POST {routePath}. With IncludeViPaqData, Returns ViPaqData Per Packed Bin")]
	public async Task Post_WithIncludeViPaqData_ReturnsViPaqData()
	{
		var request = new PackCustomCompareRequest
		{
			Parameters = new() { Algorithm = Algorithm.FFD, IncludeViPaqData = true },
			Bins = this.sampleRequest.Bins,
			Items = this.sampleRequest.Items
		};
		await base.PackCompareRequest_Validate(routePath, request, result =>
		{
			foreach (var binResult in result.Results.Where(x => x.PackedItems?.Count > 0))
				binResult.ViPaqData.ShouldNotBeNullOrEmpty();
		});
	}

	#endregion
}
