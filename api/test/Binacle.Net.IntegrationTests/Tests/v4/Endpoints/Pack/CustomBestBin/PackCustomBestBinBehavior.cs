using Binacle.Net.IntegrationTests.v4.Abstractions;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Pack;

namespace Binacle.Net.IntegrationTests.v4.Endpoints.Pack.CustomBestBin;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class PackCustomBestBinBehavior : BehaviourTestsBase
{
	private const string routePath = "/api/v4/pack/best-bin";

	private readonly PackCustomBestBinRequest sampleRequest = new()
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

	public PackCustomBestBinBehavior(BinacleApi sut) : base(sut)
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
		=> base.PackRequest_Validate(routePath, this.sampleRequest,
			result => result.Bin.ID.ShouldBeOneOf("bin_small", "bin_medium", "bin_large"));

	[Fact(DisplayName = $"POST {routePath}. With Algorithm FFD, Returns AlgorithmUsed FFD")]
	public async Task Post_WithAlgorithmFFD_ReturnsAlgorithmUsedFFD()
	{
		var request = new PackCustomBestBinRequest
		{
			Parameters = new() { Algorithm = Algorithm.FFD },
			Bins = this.sampleRequest.Bins,
			Items = this.sampleRequest.Items
		};
		await base.PackRequest_Validate(routePath, request, result => result.AlgorithmUsed.ShouldBe("FFD"));
	}

	// The distinguishing behaviour: when nothing packs fully, best-bin takes the highest utilization while
	// smallest-bin would take the smaller bin. bin_dense packs 8 cubes (85.03%), bin_small only 1 (72.90%).
	[Fact(DisplayName = $"POST {routePath}. When Nothing Fully Packs, Returns The Highest Utilization Bin")]
	public async Task Post_WhenNothingFullyPacks_ReturnsHighestUtilizationBin()
	{
		var request = new PackCustomBestBinRequest
		{
			Parameters = new() { Algorithm = Algorithm.FFD },
			Bins =
			[
				new() { ID = "bin_small", Length = 10, Width = 10, Height = 10 },
				new() { ID = "bin_dense", Length = 19, Width = 19, Height = 19 },
			],
			Items =
			[
				new() { ID = "cube", Quantity = 9, Length = 9, Width = 9, Height = 9 },
				new() { ID = "huge", Quantity = 1, Length = 50, Width = 50, Height = 50 },
			]
		};
		await base.PackRequest_Validate(routePath, request, result =>
		{
			result.Status.ShouldBe(BinPackResultStatus.PartiallyPacked);
			result.Bin.ID.ShouldBe("bin_dense");
		});
	}

	[Fact(DisplayName = $"POST {routePath}. With All Bins Too Small, Returns NotPacked")]
	public async Task Post_WithAllBinsTooSmall_Returns_NotPacked()
	{
		var request = new PackCustomBestBinRequest
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

	[Fact(DisplayName = $"POST {routePath}. Without IncludeViPaqData, Returns No ViPaqData")]
	public Task Post_WithoutIncludeViPaqData_Returns_NoViPaqData()
		=> base.PackRequest_Validate(routePath, this.sampleRequest,
			result => result.ViPaqData.ShouldBeNull());

	#endregion
}
