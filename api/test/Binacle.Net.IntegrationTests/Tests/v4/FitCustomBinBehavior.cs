using Binacle.Net.IntegrationTests.v4.Abstractions;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Fit;

namespace Binacle.Net.IntegrationTests.v4;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class FitCustomBinBehavior : BehaviourTestsBase
{
	private const string routePath = "/api/v4/fit/bin";

	private readonly FitCustomBinRequest sampleRequest = new()
	{
		Parameters = new() { Algorithm = Algorithm.Best },
		Bin = new() { ID = "custom_bin", Length = 30, Width = 40, Height = 60 },
		Items =
		[
			new() { ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
			new() { ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
			new() { ID = "box_3", Quantity = 1, Length = 12, Width = 10, Height = 15 },
		]
	};

	public FitCustomBinBehavior(BinacleApi sut) : base(sut)
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

	[Fact(DisplayName = $"POST {routePath}. With Zero Dimension On Item, Returns 422 UnprocessableContent")]
	public async Task Post_WithZeroDimensionOnItem_Returns_422UnprocessableContent()
	{
		this.sampleRequest.Items!.First(x => x.ID == "box_2").Length = 0;
		await base.Request_Returns_422UnprocessableContent(routePath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Zero Dimension On Bin, Returns 422 UnprocessableContent")]
	public async Task Post_WithZeroDimensionOnBin_Returns_422UnprocessableContent()
	{
		this.sampleRequest.Bin!.Length = 0;
		await base.Request_Returns_422UnprocessableContent(routePath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Excessive Item Dimension, Returns 422 UnprocessableContent")]
	public async Task Post_WithExcessiveItemDimension_Returns_422UnprocessableContent()
	{
		this.sampleRequest.Items!.First().Length = 65536;
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
		var request = new FitCustomBinRequest
		{
			Parameters = new() { Algorithm = Algorithm.FFD },
			Bin = this.sampleRequest.Bin,
			Items = this.sampleRequest.Items
		};
		await base.FitRequest_Validate(routePath, request, result => result.AlgorithmUsed.ShouldBe("FFD"));
	}

	[Fact(DisplayName = $"POST {routePath}. With Algorithm WFD, Returns AlgorithmUsed WFD")]
	public async Task Post_WithAlgorithmWFD_ReturnsAlgorithmUsedWFD()
	{
		var request = new FitCustomBinRequest
		{
			Parameters = new() { Algorithm = Algorithm.WFD },
			Bin = this.sampleRequest.Bin,
			Items = this.sampleRequest.Items
		};
		await base.FitRequest_Validate(routePath, request, result => result.AlgorithmUsed.ShouldBe("WFD"));
	}

	[Fact(DisplayName = $"POST {routePath}. With Algorithm BFD, Returns AlgorithmUsed BFD")]
	public async Task Post_WithAlgorithmBFD_ReturnsAlgorithmUsedBFD()
	{
		var request = new FitCustomBinRequest
		{
			Parameters = new() { Algorithm = Algorithm.BFD },
			Bin = this.sampleRequest.Bin,
			Items = this.sampleRequest.Items
		};
		await base.FitRequest_Validate(routePath, request, result => result.AlgorithmUsed.ShouldBe("BFD"));
	}

	[Fact(DisplayName = $"POST {routePath}. With Algorithm Best, Returns AlgorithmUsed")]
	public Task Post_WithAlgorithmBest_ReturnsAlgorithmUsed()
		=> base.FitRequest_Validate(routePath, this.sampleRequest,
			result => result.AlgorithmUsed.ShouldNotBeNullOrEmpty()
		);

	[Fact(DisplayName = $"POST {routePath}. When All Items Fit, Returns Fits")]
	public Task Post_WhenAllItemsFit_Returns_Fits()
		=> base.FitRequest_Validate(routePath, this.sampleRequest, result =>
		{
			result.Status.ShouldBe(BinFitResultStatus.Fits);
			result.EarlyExitReason.ShouldBe(BinFitEarlyExitReason.None);
		});

	[Fact(DisplayName = $"POST {routePath}. With Large Volume, Returns EarlyExit TotalVolumeExceeded")]
	public async Task Post_WithLargeVolume_Returns_EarlyExit_TotalVolumeExceeded()
	{
		var request = CreateSpecialRequest();
		request.Items!.First(x => x.ID == "special_box_1").Quantity = 3;

		await base.FitRequest_Validate(routePath, request, result =>
		{
			result.Status.ShouldBe(BinFitResultStatus.EarlyExit);
			result.EarlyExitReason.ShouldBe(BinFitEarlyExitReason.ContainerVolumeExceeded);
		});
	}

	[Fact(DisplayName = $"POST {routePath}. With Large Dimension, Returns EarlyExit ContainerDimensionExceeded")]
	public async Task Post_WithLargeDimension_Returns_EarlyExit_ContainerDimensionExceeded()
	{
		var request = CreateSpecialRequest();
		var specialBox = request.Items!.First(x => x.ID == "special_box_1");
		specialBox.Length = 61;
		specialBox.Width = 5;
		specialBox.Height = 5;

		await base.FitRequest_Validate(routePath, request, result =>
		{
			result.Status.ShouldBe(BinFitResultStatus.EarlyExit);
			result.EarlyExitReason.ShouldBe(BinFitEarlyExitReason.ContainerDimensionExceeded);
		});
	}

	[Fact(DisplayName = $"POST {routePath}. Without IncludeViPaqData, Returns No ViPaqData")]
	public Task Post_WithoutIncludeViPaqData_Returns_NoViPaqData()
		=> base.FitRequest_Validate(routePath, this.sampleRequest,
			result => result.ViPaqData.ShouldBeNull());

	[Fact(DisplayName = $"POST {routePath}. With IncludeViPaqData, Returns ViPaqData When Items Fit")]
	public async Task Post_WithIncludeViPaqData_ReturnsViPaqData()
	{
		var request = CreateSpecialRequest(p => p.IncludeViPaqData = true);

		await base.FitRequest_Validate(routePath, request, result =>
		{
			if (result.Status == BinFitResultStatus.Fits)
				result.ViPaqData.ShouldNotBeNullOrEmpty();
		});
	}

	[Fact(DisplayName = $"POST {routePath}. Packed Items Include Coordinates")]
	public async Task Post_PackedItems_IncludeCoordinates()
	{
		await base.FitRequest_Validate(routePath, this.sampleRequest, result =>
		{
			foreach (var item in result.PackedItems!)
			{
				item.X.ShouldBeGreaterThanOrEqualTo(0);
				item.Y.ShouldBeGreaterThanOrEqualTo(0);
				item.Z.ShouldBeGreaterThanOrEqualTo(0);
			}
		});
	}

	[Fact(DisplayName = $"POST {routePath}. Response Bin Matches Request Bin")]
	public Task Post_ResponseBinMatchesRequestBin()
		=> base.FitRequest_Validate(routePath, this.sampleRequest, result =>
		{
			result.Bin.ID.ShouldBe(this.sampleRequest.Bin!.ID);
			result.Bin.Length.ShouldBe(this.sampleRequest.Bin!.Length);
			result.Bin.Width.ShouldBe(this.sampleRequest.Bin!.Width);
			result.Bin.Height.ShouldBe(this.sampleRequest.Bin!.Height);
		});

	#endregion

	private FitCustomBinRequest CreateSpecialRequest(Action<OperationParameters>? modifyParameters = null)
	{
		var request = new FitCustomBinRequest
		{
			Parameters = new() { Algorithm = Algorithm.FFD },
			Bin = new() { ID = "special_bin", Length = 10, Width = 40, Height = 60 },
			Items =
			[
				new() { ID = "special_box_1", Quantity = 1, Length = 8, Width = 40, Height = 60 },
				new() { ID = "box_1", Quantity = 1, Length = 5, Width = 5, Height = 5 },
			]
		};
		modifyParameters?.Invoke(request.Parameters!);
		return request;
	}
}
