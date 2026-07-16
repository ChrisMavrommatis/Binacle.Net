using Binacle.Net.IntegrationTests.v4.Abstractions;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Fit;
using Binacle.TestsKernel.Algorithms.Providers;

namespace Binacle.Net.IntegrationTests.v4.Endpoints.Fit.PresetSmallestBin;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class FitPresetSmallestBinBehavior : BehaviourTestsBase
{
	private const string routePath = "/api/v4/fit/smallest-bin/{preset}";

	private static string UrlFor(string preset) => routePath.Replace("{preset}", preset);

	private readonly FitPresetSmallestBinRequest sampleRequest = new()
	{
		Parameters = new() { Algorithm = Algorithm.Best },
		Items =
		[
			new() { ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
			new() { ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
			new() { ID = "box_3", Quantity = 1, Length = 12, Width = 10, Height = 15 },
		]
	};

	public FitPresetSmallestBinBehavior(BinacleApi sut) : base(sut)
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

	[Fact(DisplayName = $"POST {routePath}. With Duplicate Item IDs, Returns 422 UnprocessableContent")]
	public async Task Post_WithDuplicateItemIds_Returns_422UnprocessableContent()
	{
		foreach (var item in this.sampleRequest.Items!)
			item.ID = "box_1";
		await base.Request_Returns_422UnprocessableContent(UrlFor(PresetKeys.CustomProblems), this.sampleRequest);
	}

	#endregion

	#region Response Data

	[Fact(DisplayName = $"POST {routePath}. Returns A Single Bin From The Preset")]
	public Task Post_ReturnsSingleBinFromPreset()
		=> base.FitRequest_Validate(UrlFor(PresetKeys.CustomProblems), this.sampleRequest,
			result => CustomProblemsScenarioProvider.GetDistinctBinIds().ShouldContain(result.Bin.ID));

	[Fact(DisplayName = $"POST {routePath}. With Algorithm FFD, Returns AlgorithmUsed FFD")]
	public async Task Post_WithAlgorithmFFD_ReturnsAlgorithmUsedFFD()
	{
		var request = new FitPresetSmallestBinRequest
		{
			Parameters = new() { Algorithm = Algorithm.FFD },
			Items = this.sampleRequest.Items
		};
		await base.FitRequest_Validate(UrlFor(PresetKeys.CustomProblems), request,
			result => result.AlgorithmUsed.ShouldBe("FFD"));
	}

	// A 1x1x1 item fits every bin in the preset, so the least roomy one has to win outright.
	[Fact(DisplayName = $"POST {routePath}. Returns The Smallest Bin The Items Fit Into")]
	public async Task Post_ReturnsSmallestBinThatFits()
	{
		var request = new FitPresetSmallestBinRequest
		{
			Parameters = new() { Algorithm = Algorithm.FFD },
			Items = [new() { ID = "tiny_box", Quantity = 1, Length = 1, Width = 1, Height = 1 }]
		};
		await base.FitRequest_Validate(UrlFor(PresetKeys.CustomProblems), request, result =>
		{
			result.Bin.ID.ShouldBe(CustomProblemsScenarioProvider.GetSmallestBin().ID);
			result.Status.ShouldBe(BinFitResultStatus.Fits);
		});
	}

	[Fact(DisplayName = $"POST {routePath}. Without IncludeViPaqData, Returns No ViPaqData")]
	public Task Post_WithoutIncludeViPaqData_Returns_NoViPaqData()
		=> base.FitRequest_Validate(UrlFor(PresetKeys.CustomProblems), this.sampleRequest,
			result => result.ViPaqData.ShouldBeNull());

	#endregion
}
