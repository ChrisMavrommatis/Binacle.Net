using Binacle.Net.IntegrationTests.v4.Abstractions;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Pack;

namespace Binacle.Net.IntegrationTests.v4;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class PackSmallestBinBehavior : BehaviourTestsBase
{
	private const string routePath = "/api/v4/pack/smallest-bin";

	private readonly PackCustomSmallestBinRequest sampleRequest = new()
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

	public PackSmallestBinBehavior(BinacleApi sut) : base(sut)
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

	[Fact(DisplayName = $"POST {routePath}. With Zero Dimension On Item, Returns 422 UnprocessableContent")]
	public async Task Post_WithZeroDimensionOnItem_Returns_422UnprocessableContent()
	{
		this.sampleRequest.Items!.First(x => x.ID == "box_2").Length = 0;
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

	[Fact(DisplayName = $"POST {routePath}. With Algorithm FFD, Returns AlgorithmUsed FFD")]
	public async Task Post_WithAlgorithmFFD_ReturnsAlgorithmUsedFFD()
	{
		var request = new PackCustomSmallestBinRequest
		{
			Parameters = new() { Algorithm = Algorithm.FFD },
			Bins = this.sampleRequest.Bins,
			Items = this.sampleRequest.Items
		};
		await base.PackRequest_Validate(routePath, request, result => result.AlgorithmUsed.ShouldBe("FFD"));
	}

	[Fact(DisplayName = $"POST {routePath}. With Algorithm WFD, Returns AlgorithmUsed WFD")]
	public async Task Post_WithAlgorithmWFD_ReturnsAlgorithmUsedWFD()
	{
		var request = new PackCustomSmallestBinRequest
		{
			Parameters = new() { Algorithm = Algorithm.WFD },
			Bins = this.sampleRequest.Bins,
			Items = this.sampleRequest.Items
		};
		await base.PackRequest_Validate(routePath, request, result => result.AlgorithmUsed.ShouldBe("WFD"));
	}

	[Fact(DisplayName = $"POST {routePath}. With Algorithm BFD, Returns AlgorithmUsed BFD")]
	public async Task Post_WithAlgorithmBFD_ReturnsAlgorithmUsedBFD()
	{
		var request = new PackCustomSmallestBinRequest
		{
			Parameters = new() { Algorithm = Algorithm.BFD },
			Bins = this.sampleRequest.Bins,
			Items = this.sampleRequest.Items
		};
		await base.PackRequest_Validate(routePath, request, result => result.AlgorithmUsed.ShouldBe("BFD"));
	}

	[Fact(DisplayName = $"POST {routePath}. Returns Single Bin Result")]
	public Task Post_ReturnsSingleBinResult()
		=> base.PackRequest_Validate(routePath, this.sampleRequest,
			result => result.Bin.ID.ShouldBeOneOf("bin_small", "bin_medium", "bin_large")
		);

	[Fact(DisplayName = $"POST {routePath}. Returns Smallest Bin That Packs All Items")]
	public async Task Post_ReturnsSmallestBinThatPacks()
	{
		// 11x11x11 can't fit in bin_small (L=10) but fits in bin_medium (L=20)
		var request = new PackCustomSmallestBinRequest
		{
			Parameters = new() { Algorithm = Algorithm.FFD },
			Bins = this.sampleRequest.Bins,
			Items = [new() { ID = "medium_box", Quantity = 1, Length = 11, Width = 11, Height = 11 }]
		};
		await base.PackRequest_Validate(routePath, request, result =>
		{
			result.Bin.ID.ShouldBe("bin_medium");
			result.Status.ShouldBe(BinPackResultStatus.FullyPacked);
		});
	}

	[Fact(DisplayName = $"POST {routePath}. With All Bins Too Small, Returns NotPacked")]
	public async Task Post_WithAllBinsTooSmall_Returns_NotPacked()
	{
		var request = new PackCustomSmallestBinRequest
		{
			Parameters = new() { Algorithm = Algorithm.FFD },
			Bins =
			[
				new() { ID = "tiny_bin_1", Length = 1, Width = 1, Height = 1 },
				new() { ID = "tiny_bin_2", Length = 2, Width = 2, Height = 2 },
			],
			Items = [new() { ID = "big_box", Quantity = 1, Length = 10, Width = 10, Height = 10 }]
		};
		await base.PackRequest_Validate(routePath, request,
			result => result.Status.ShouldBe(BinPackResultStatus.NotPacked));
	}

	[Fact(DisplayName = $"POST {routePath}. Packed Items Include Coordinates")]
	public async Task Post_PackedItems_IncludeCoordinates()
	{
		await base.PackRequest_Validate(routePath, this.sampleRequest, result =>
		{
			foreach (var item in result.PackedItems!)
			{
				item.X.ShouldBeGreaterThanOrEqualTo(0);
				item.Y.ShouldBeGreaterThanOrEqualTo(0);
				item.Z.ShouldBeGreaterThanOrEqualTo(0);
			}
		});
	}

	[Fact(DisplayName = $"POST {routePath}. Without IncludeViPaqData, Returns No ViPaqData")]
	public Task Post_WithoutIncludeViPaqData_Returns_NoViPaqData()
		=> base.PackRequest_Validate(routePath, this.sampleRequest,
			result => result.ViPaqData.ShouldBeNull());

	[Fact(DisplayName = $"POST {routePath}. With IncludeViPaqData, Returns ViPaqData When Items Packed")]
	public async Task Post_WithIncludeViPaqData_ReturnsViPaqData()
	{
		var request = new PackCustomSmallestBinRequest
		{
			Parameters = new() { Algorithm = Algorithm.FFD, IncludeViPaqData = true },
			Bins = this.sampleRequest.Bins,
			Items = this.sampleRequest.Items
		};
		await base.PackRequest_Validate(routePath, request, result =>
		{
			if (result.Status == BinPackResultStatus.FullyPacked || result.Status == BinPackResultStatus.PartiallyPacked)
				result.ViPaqData.ShouldNotBeNullOrEmpty();
		});
	}

	[Fact(DisplayName = $"POST {routePath}. Reports All Items Across Packed And Unpacked")]
	public async Task Post_ReportsAllItems()
	{
		await base.PackRequest_Validate(routePath, this.sampleRequest, result =>
		{
			var totalItems = (result.PackedItems?.Count ?? 0)
			                 + (result.UnpackedItems?.Sum(x => x.Quantity) ?? 0);
			totalItems.ShouldBeGreaterThan(0);
		});
	}

	#endregion
}
