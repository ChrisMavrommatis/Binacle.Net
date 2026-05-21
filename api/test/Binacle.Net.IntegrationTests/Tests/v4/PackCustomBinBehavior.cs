using Binacle.Net.IntegrationTests.v4.Abstractions;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Pack;

namespace Binacle.Net.IntegrationTests.v4;
// TODO: Review
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class PackCustomBinBehavior : BehaviourTestsBase
{
	private const string routePath = "/api/v4/pack/bin";

	private readonly PackCustomBinRequest sampleRequest = new()
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

	public PackCustomBinBehavior(BinacleApi sut) : base(sut)
	{
	}

	#region Response Statuses

	[Fact(DisplayName = $"POST {routePath}. With Valid Request, Returns 200 OK")]
	public Task Post_WithValidRequest_Returns_200Ok()
		=> base.Request_Returns_200Ok(routePath, this.sampleRequest);

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

	[Fact(DisplayName = $"POST {routePath}. With Duplicate Item IDs, Returns 422 UnprocessableContent")]
	public async Task Post_WithDuplicateItemIds_Returns_422UnprocessableContent()
	{
		foreach (var item in this.sampleRequest.Items!)
			item.ID = "box_1";
		await base.Request_Returns_422UnprocessableContent(routePath, this.sampleRequest);
	}

	#endregion

	#region Response Data

	[Fact(DisplayName = $"POST {routePath}. With Algorithm Best, Returns AlgorithmUsed")]
	public Task Post_WithAlgorithmBest_ReturnsAlgorithmUsed()
		=> base.PackRequest_Validate(routePath, this.sampleRequest,
			result => result.AlgorithmUsed.ShouldNotBeNullOrEmpty()
		);

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

	[Fact(DisplayName = $"POST {routePath}. With IncludeViPaqData, Returns ViPaqData When Items Packed")]
	public async Task Post_WithIncludeViPaqData_ReturnsViPaqData()
	{
		var request = CreateSpecialRequest(p => p.IncludeViPaqData = true);

		await base.PackRequest_Validate(routePath, request, result =>
		{
			if (result.Status == BinPackResultStatus.FullyPacked || result.Status == BinPackResultStatus.PartiallyPacked)
				result.ViPaqData.ShouldNotBeNullOrEmpty();
		});
	}

	[Fact(DisplayName = $"POST {routePath}. Reports All Items Across Packed And Unpacked")]
	public async Task Post_ReportsAllItems()
	{
		var request = CreateSpecialRequest();

		await base.PackRequest_Validate(routePath, request, result =>
		{
			var totalItems = (result.PackedItems?.Count ?? 0)
			                 + (result.UnpackedItems?.Sum(x => x.Quantity) ?? 0);
			totalItems.ShouldBeGreaterThan(0);
		});
	}

	#endregion

	private PackCustomBinRequest CreateSpecialRequest(Action<OperationParameters>? modifyParameters = null)
	{
		var request = new PackCustomBinRequest
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
