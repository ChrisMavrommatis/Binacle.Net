using Binacle.Net.Configuration;
using Binacle.Net.IntegrationTests.v4.Abstractions;
using Binacle.Net.v4.Contracts;
using Binacle.Net.v4.Contracts.Fit;
using Binacle.TestsKernel.Algorithms.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.IntegrationTests.v4;
// TODO: Review
[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class FitPresetBinBehavior : BehaviourTestsBase
{
	private const string routePath = "/api/v4/fit/bin/{preset}/{bin}";

	private readonly IOptions<BinPresetOptions> presetOptions;
	private readonly string validBinId;

	private readonly FitPresetBinRequest sampleRequest = new()
	{
		Parameters = new() { Algorithm = Algorithm.Best },
		Items =
		[
			new() { ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
			new() { ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
			new() { ID = "box_3", Quantity = 1, Length = 12, Width = 10, Height = 15 },
		]
	};

	public FitPresetBinBehavior(BinacleApi sut) : base(sut)
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

	[Fact(DisplayName = $"POST {routePath}. With Duplicate Item IDs, Returns 422 UnprocessableContent")]
	public async Task Post_WithDuplicateItemIds_Returns_422UnprocessableContent()
	{
		foreach (var item in this.sampleRequest.Items!)
			item.ID = "box_1";
		var url = routePath
			.Replace("{preset}", PresetKeys.CustomProblems)
			.Replace("{bin}", this.validBinId);
		await base.Request_Returns_422UnprocessableContent(url, this.sampleRequest);
	}

	#endregion

	#region Response Data

	[Fact(DisplayName = $"POST {routePath}. With Large Volume, Returns EarlyExit TotalVolumeExceeded")]
	public async Task Post_WithLargeVolume_Returns_EarlyExit_TotalVolumeExceeded()
	{
		var request = CreateSpecialRequest(p => p.Algorithm = Algorithm.FFD);
		request.Items!.First(x => x.ID == "special_box_1").Quantity = 3;

		var url = routePath
			.Replace("{preset}", PresetKeys.SpecialSet)
			.Replace("{bin}", "special_bin_1");

		await base.FitRequest_Validate(url, request, result =>
		{
			result.Status.ShouldBe(BinFitResultStatus.EarlyExit);
			result.EarlyExitReason.ShouldBe(BinFitEarlyExitReason.ContainerVolumeExceeded);
		});
	}

	#endregion

	private FitPresetBinRequest CreateSpecialRequest(Action<OperationParameters>? modifyParameters = null)
	{
		var request = new FitPresetBinRequest
		{
			Parameters = new() { Algorithm = Algorithm.FFD },
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
