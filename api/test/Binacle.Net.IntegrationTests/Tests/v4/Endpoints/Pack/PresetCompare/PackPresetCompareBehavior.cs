using Binacle.Net.IntegrationTests.v4.Abstractions;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Pack;
using Binacle.TestsKernel.Algorithms.Providers;

namespace Binacle.Net.IntegrationTests.v4.Endpoints.Pack.PresetCompare;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class PackPresetCompareBehavior : BehaviourTestsBase
{
	private const string routePath = "/api/v4/pack/compare-bins/{preset}";

	private static string UrlFor(string preset) => routePath.Replace("{preset}", preset);

	private readonly PackPresetCompareRequest sampleRequest = new()
	{
		Parameters = new() { Algorithm = Algorithm.Best },
		Items =
		[
			new() { ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
			new() { ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
			new() { ID = "box_3", Quantity = 1, Length = 12, Width = 10, Height = 15 },
		]
	};

	public PackPresetCompareBehavior(BinacleApi sut) : base(sut)
	{
	}

	#region Response Statuses

	[Fact(DisplayName = $"POST {routePath}. With Existing Preset, Returns 200 OK")]
	public Task Post_WithExistingPreset_Returns_200Ok()
		=> base.Request_Returns_200Ok(UrlFor(PresetKeys.CustomProblems), this.sampleRequest);

	[Fact(DisplayName = $"POST {routePath}. With Non-Existing Preset, Returns 404 NotFound")]
	public Task Post_WithNonExistingPreset_Returns_404NotFound()
		=> base.Request_Returns_404NotFound(UrlFor("non-existing-preset"), this.sampleRequest);

	[Fact(DisplayName = $"POST {routePath}. Without Algorithm, Returns 422 UnprocessableContent")]
	public async Task Post_WithoutAlgorithm_Returns_422UnprocessableContent()
	{
		this.sampleRequest.Parameters!.Algorithm = null;
		await base.Request_Returns_422UnprocessableContent(UrlFor(PresetKeys.CustomProblems), this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. Without Items, Returns 422 UnprocessableContent")]
	public async Task Post_WithoutItems_Returns_422UnprocessableContent()
	{
		this.sampleRequest.Items = [];
		await base.Request_Returns_422UnprocessableContent(UrlFor(PresetKeys.CustomProblems), this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Zero Dimension On Item, Returns 422 UnprocessableContent")]
	public async Task Post_WithZeroDimensionOnItem_Returns_422UnprocessableContent()
	{
		this.sampleRequest.Items!.First(x => x.ID == "box_2").Length = 0;
		await base.Request_Returns_422UnprocessableContent(UrlFor(PresetKeys.CustomProblems), this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Duplicate Item IDs, Returns 422 UnprocessableContent")]
	public async Task Post_WithDuplicateItemIds_Returns_422UnprocessableContent()
	{
		foreach (var item in this.sampleRequest.Items!)
			item.ID = "box_1";
		await base.Request_Returns_422UnprocessableContent(UrlFor(PresetKeys.CustomProblems), this.sampleRequest);
	}

	#endregion

	#region Response Data

	[Fact(DisplayName = $"POST {routePath}. Returns One Result Per Bin In The Preset")]
	public Task Post_ReturnsOneResultPerPresetBin()
		=> base.PackCompareRequest_Validate(UrlFor(PresetKeys.CustomProblems), this.sampleRequest, result =>
			result.Results.Select(x => x.Bin.ID)
				.ShouldBe(CustomProblemsScenarioProvider.GetDistinctBinIds()));

	[Fact(DisplayName = $"POST {routePath}. With Algorithm FFD, Returns AlgorithmUsed FFD For Every Bin")]
	public async Task Post_WithAlgorithmFFD_ReturnsAlgorithmUsedFFD()
	{
		var request = new PackPresetCompareRequest
		{
			Parameters = new() { Algorithm = Algorithm.FFD },
			Items = this.sampleRequest.Items
		};
		await base.PackCompareRequest_Validate(UrlFor(PresetKeys.CustomProblems), request, result =>
		{
			foreach (var binResult in result.Results)
				binResult.AlgorithmUsed.ShouldBe("FFD");
		});
	}

	[Fact(DisplayName = $"POST {routePath}. Without IncludeViPaqData, Returns No ViPaqData")]
	public Task Post_WithoutIncludeViPaqData_Returns_NoViPaqData()
		=> base.PackCompareRequest_Validate(UrlFor(PresetKeys.CustomProblems), this.sampleRequest, result =>
		{
			foreach (var binResult in result.Results)
				binResult.ViPaqData.ShouldBeNull();
		});

	[Fact(DisplayName = $"POST {routePath}. With IncludeViPaqData, Returns ViPaqData Per Packed Bin")]
	public async Task Post_WithIncludeViPaqData_ReturnsViPaqData()
	{
		var request = new PackPresetCompareRequest
		{
			Parameters = new() { Algorithm = Algorithm.FFD, IncludeViPaqData = true },
			Items = this.sampleRequest.Items
		};
		await base.PackCompareRequest_Validate(UrlFor(PresetKeys.CustomProblems), request, result =>
		{
			foreach (var binResult in result.Results.Where(x => x.PackedItems?.Count > 0))
				binResult.ViPaqData.ShouldNotBeNullOrEmpty();
		});
	}

	#endregion
}
