
using Binacle.Net.Models;

namespace Binacle.Net.IntegrationTests.v3;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class PackByCustomBehavior : Abstractions.BehaviourTestsBase
{
	private readonly Binacle.Net.v3.Contracts.PackByCustomRequest sampleRequest = new()
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

	
	private const string routePath = "/api/v3/pack/by-custom";

	public PackByCustomBehavior(BinacleApi sut) : base(sut)
	{
	}

	#region Response Statuses

	[Fact(DisplayName = $"POST {routePath}. With Valid Request, Returns 200 OK")]
	public Task Post_WithValidRequest_Returns200Ok()
		=> base.Request_Returns_200Ok(routePath, this.sampleRequest);

	[Fact(DisplayName = $"POST {routePath}. With Zero Dimension On Item, Returns 422 UnprocessableContent")]
	public async Task Post_WithZeroDimensionOnItem_Returns422UnprocessableContent()
	{
		this.sampleRequest.Items!.FirstOrDefault(x => x.ID == "box_2")!.Length = 0;
		await base.Request_Returns_422UnprocessableContent(routePath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Zero Dimension On Bin, Returns 422 UnprocessableContent")]
	public async Task Post_WithZeroDimensionOnBin_Returns422UnprocessableContent()
	{
		this.sampleRequest.Bins!.FirstOrDefault(x => x.ID == "custom_bin_1")!.Length = 0;
		await base.Request_Returns_422UnprocessableContent(routePath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Same Id On Bins, Returns 422 UnprocessableContent")]
	public async Task Post_WithSameIdOnBins_Returns422UnprocessableContent()
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
	
	[Fact(DisplayName = $"POST {routePath}. With Default Parameters, Reports All Items")]
	public async Task Post_WithDefaultParameters_ReportsAllItems()
	{
		var request = this.CreateSpecialRequest();

		await base.PackRequest_ValidateBasedOnParameters(
			routePath,
			request,
			result =>
			{
				result.Data.ShouldHaveCount(request.Bins!.Count);
			}
		);
	}
	
	private Binacle.Net.v3.Contracts.PackByCustomRequest CreateSpecialRequest(
		Action<Binacle.Net.v3.Contracts.PackRequestParameters>? modifyParameters = null
	)
	{
		var request = new Binacle.Net.v3.Contracts.PackByCustomRequest
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
