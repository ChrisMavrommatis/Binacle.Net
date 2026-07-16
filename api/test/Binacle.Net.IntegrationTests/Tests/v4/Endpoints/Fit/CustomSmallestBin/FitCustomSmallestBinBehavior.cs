using Binacle.Net.IntegrationTests.v4.Abstractions;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Fit;

namespace Binacle.Net.IntegrationTests.v4.Endpoints.Fit.CustomSmallestBin;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class FitCustomSmallestBinBehavior : BehaviourTestsBase
{
	private const string routePath = "/api/v4/fit/smallest-bin";

	private readonly FitCustomSmallestBinRequest sampleRequest = new()
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

	public FitCustomSmallestBinBehavior(BinacleApi sut) : base(sut)
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

	[Fact(DisplayName = $"POST {routePath}. Without Bins, Returns 422 UnprocessableContent")]
	public async Task Post_WithoutBins_Returns_422UnprocessableContent()
	{
		this.sampleRequest.Bins = [];
		await base.Request_Returns_422UnprocessableContent(routePath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. Without Items, Returns 422 UnprocessableContent")]
	public async Task Post_WithoutItems_Returns_422UnprocessableContent()
	{
		this.sampleRequest.Items = [];
		await base.Request_Returns_422UnprocessableContent(routePath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Duplicate Bin IDs, Returns 422 UnprocessableContent")]
	public async Task Post_WithDuplicateBinIds_Returns_422UnprocessableContent()
	{
		foreach (var bin in this.sampleRequest.Bins!)
			bin.ID = "bin_small";
		await base.Request_Returns_422UnprocessableContent(routePath, this.sampleRequest);
	}

	#endregion

	#region Response Data

	[Fact(DisplayName = $"POST {routePath}. Returns A Single Bin From The Request")]
	public Task Post_ReturnsSingleBinResult()
		=> base.FitRequest_Validate(routePath, this.sampleRequest,
			result => result.Bin.ID.ShouldBeOneOf("bin_small", "bin_medium", "bin_large"));

	[Fact(DisplayName = $"POST {routePath}. With Algorithm FFD, Returns AlgorithmUsed FFD")]
	public async Task Post_WithAlgorithmFFD_ReturnsAlgorithmUsedFFD()
	{
		var request = new FitCustomSmallestBinRequest
		{
			Parameters = new() { Algorithm = Algorithm.FFD },
			Bins = this.sampleRequest.Bins,
			Items = this.sampleRequest.Items
		};
		await base.FitRequest_Validate(routePath, request, result => result.AlgorithmUsed.ShouldBe("FFD"));
	}

	[Fact(DisplayName = $"POST {routePath}. Returns The Smallest Bin The Items Fit Into")]
	public async Task Post_ReturnsSmallestBinThatFits()
	{
		// 11x11x11 does not fit bin_small (L=10) but does fit bin_medium (L=20).
		var request = new FitCustomSmallestBinRequest
		{
			Parameters = new() { Algorithm = Algorithm.FFD },
			Bins = this.sampleRequest.Bins,
			Items = [new() { ID = "medium_box", Quantity = 1, Length = 11, Width = 11, Height = 11 }]
		};
		await base.FitRequest_Validate(routePath, request, result =>
		{
			result.Bin.ID.ShouldBe("bin_medium");
			result.Status.ShouldBe(BinFitResultStatus.Fits);
		});
	}

	[Fact(DisplayName = $"POST {routePath}. With All Bins Too Small, Does Not Return Fits")]
	public async Task Post_WithAllBinsTooSmall_DoesNotReturn_Fits()
	{
		var request = new FitCustomSmallestBinRequest
		{
			Parameters = new() { Algorithm = Algorithm.FFD },
			Bins =
			[
				new() { ID = "tiny_bin_1", Length = 1, Width = 1, Height = 1 },
				new() { ID = "tiny_bin_2", Length = 2, Width = 2, Height = 2 },
			],
			Items = [new() { ID = "big_box", Quantity = 1, Length = 10, Width = 10, Height = 10 }]
		};
		await base.FitRequest_Validate(routePath, request, result =>
			result.Status.ShouldBeOneOf(BinFitResultStatus.DoesNotFit, BinFitResultStatus.EarlyExit));
	}

	[Fact(DisplayName = $"POST {routePath}. Without IncludeViPaqData, Returns No ViPaqData")]
	public Task Post_WithoutIncludeViPaqData_Returns_NoViPaqData()
		=> base.FitRequest_Validate(routePath, this.sampleRequest,
			result => result.ViPaqData.ShouldBeNull());

	#endregion
}
