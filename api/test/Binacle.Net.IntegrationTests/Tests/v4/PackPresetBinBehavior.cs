using Binacle.Net.Configuration;
using Binacle.Net.IntegrationTests.v4.Abstractions;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Pack;
using Binacle.TestsKernel.Algorithms.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.IntegrationTests.v4;
// TODO: Review
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class PackPresetBinBehavior : BehaviourTestsBase
{
	private const string routePath = "/api/v4/pack/bin/{preset}/{bin}";

	private readonly IOptions<BinPresetOptions> presetOptions;
	private readonly string validBinId;

	private readonly PackPresetBinRequest sampleRequest = new()
	{
		Parameters = new() { Algorithm = Algorithm.Best },
		Items =
		[
			new() { ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
			new() { ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
			new() { ID = "box_3", Quantity = 1, Length = 12, Width = 10, Height = 15 },
		]
	};

	public PackPresetBinBehavior(BinacleApi sut) : base(sut)
	{
		this.presetOptions = this.Sut.Services.GetRequiredService<IOptions<BinPresetOptions>>();
		this.validBinId = CustomProblemsScenarioProvider.GetScenarios().First().Bin.ID;
	}

	#region Response Statuses

	[Fact(DisplayName = $"POST {routePath}. With Existing Preset And Bin, Returns 200 OK")]
	public async Task Post_WithExistingPresetAndBin_Returns_200Ok()
	{
		var url = routePath
			.Replace("{preset}", PresetKeys.CustomProblems)
			.Replace("{bin}", this.validBinId);
		await base.Request_Returns_200Ok(url, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Non-Existing Preset, Returns 404 NotFound")]
	public async Task Post_WithNonExistingPreset_Returns_404NotFound()
	{
		var url = routePath
			.Replace("{preset}", "non-existing-preset")
			.Replace("{bin}", this.validBinId);
		await base.Request_Returns_404NotFound(url, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Non-Existing Bin In Preset, Returns 404 NotFound")]
	public async Task Post_WithNonExistingBinInPreset_Returns_404NotFound()
	{
		var url = routePath
			.Replace("{preset}", PresetKeys.CustomProblems)
			.Replace("{bin}", "non-existing-bin");
		await base.Request_Returns_404NotFound(url, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Zero Dimension On Item, Returns 422 UnprocessableContent")]
	public async Task Post_WithZeroDimensionOnItem_Returns_422UnprocessableContent()
	{
		this.sampleRequest.Items!.First(x => x.ID == "box_2").Length = 0;
		var url = routePath
			.Replace("{preset}", PresetKeys.CustomProblems)
			.Replace("{bin}", this.validBinId);
		await base.Request_Returns_422UnprocessableContent(url, this.sampleRequest);
	}

	#endregion

	#region Response Data

	[Fact(DisplayName = $"POST {routePath}. Packed Items Include Coordinates")]
	public async Task Post_PackedItems_IncludeCoordinates()
	{
		var url = routePath
			.Replace("{preset}", PresetKeys.CustomProblems)
			.Replace("{bin}", this.validBinId);

		await base.PackRequest_Validate(url, this.sampleRequest, result =>
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
		var request = new PackPresetBinRequest
		{
			Parameters = new() { Algorithm = Algorithm.FFD, IncludeViPaqData = true },
			Items = this.sampleRequest.Items
		};

		var url = routePath
			.Replace("{preset}", PresetKeys.CustomProblems)
			.Replace("{bin}", this.validBinId);

		await base.PackRequest_Validate(url, request, result =>
		{
			if (result.Status == BinPackResultStatus.FullyPacked || result.Status == BinPackResultStatus.PartiallyPacked)
				result.ViPaqData.ShouldNotBeNullOrEmpty();
		});
	}

	#endregion
}
