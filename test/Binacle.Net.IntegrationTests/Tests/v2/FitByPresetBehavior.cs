using Binacle.Net.Configuration;
using Binacle.Net.IntegrationTests.v2.Abstractions;
using Binacle.Net.v2.Models;
using Binacle.Net.v2.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.IntegrationTests.v2;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class FitByPresetBehavior :  BehaviourTestsBase
{
	private readonly IOptions<BinPresetOptions> presetOptions;
	private readonly PresetFitRequest sampleRequest = new()
	{
		Parameters = new()
		{
			FindSmallestBinOnly = false,
			ReportFittedItems = false,
			ReportUnfittedItems = false
		},
		Items = new()
		{
			new (){ ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
			new (){ ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
			new (){ ID = "box_3", Quantity = 1, Length = 12, Width = 10, Height = 15 },
		}
	};

	private const string routePath = "/api/v2/fit/by-preset/{preset}";
	private const string validPreset = "rectangular-cuboids";

	public FitByPresetBehavior(BinacleApi sut) : base(sut)
	{
		this.presetOptions = this.Sut.Services.GetRequiredService<IOptions<BinPresetOptions>>();
	}

	#region Response Statuses
	
	[Fact(DisplayName = $"POST {routePath}. With Existing Preset And Valid Request, Returns 200 OK")]
	public async Task Post_WithExistingPresetAndValidRequest_Returns200Ok()
	{
		var urlPath = routePath.Replace("{preset}", validPreset);
		await base.Request_Returns_200Ok(urlPath, this.sampleRequest);
	}
	
	[Fact(DisplayName = $"POST {routePath}. With Non Existing Preset, Returns 404 NotFound")]
	public async Task Post_WithNonExistingPreset_Returns_404NotFound()
	{
		var urlPath = routePath.Replace("{preset}", "non-existing-preset");
		await base.Request_Returns_404NotFound(urlPath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Zero Dimensions, Returns 400 BadRequest")]
	public async Task Post_WithZeroDimensions_Returns400BadRequest()
	{
		this.sampleRequest!.Items!.FirstOrDefault(x => x.ID == "box_2")!.Length = 0;
		var urlPath = routePath.Replace("{preset}", validPreset);
		await base.Request_Returns_400BadRequest(urlPath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Same Id, Returns 400 BadRequest")]
	public async Task Post_WithSameId_Returns400BadRequest()
	{
		var urlPath = routePath.Replace("{preset}", validPreset);
		foreach (var bin in this.sampleRequest!.Items!)
		{
			bin.ID = "box_1";
		}
		await base.Request_Returns_400BadRequest(urlPath, this.sampleRequest);
	}
	
	#endregion Response Statuses
	
	#region Response Data
	
	[Fact(DisplayName = $"POST {routePath}. With Large Volume, Returns With Early Fail Total Volume Exceeded")]
	public async Task Post_WithLargeVolume_ReturnsWithEarlyFail_TotalVolumeExceeded()
	{
		var request = this.CreateSpecialRequest();
		request.Items!.FirstOrDefault(x => x.ID == "special_box_1")!.Quantity = 3;

		var preset = presetOptions.Value.Presets["special"];
		var urlPath = routePath.Replace("{preset}", "special");
		
		await base.FitRequest_ValidateBasedOnParameters(
			urlPath,
			request,
			result =>
			{
				result.Data.ShouldHaveCount(preset.Bins!.Count);
				foreach (var binFitResult in result.Data)
				{
					binFitResult.Result.ShouldBe(BinFitResultStatus.EarlyFail_TotalVolumeExceeded);
				}
			}
		);
	}

	[Fact(DisplayName = $"POST {routePath}. With Large Dimension, Returns With Early Fail Item Dimension Exceeded")]
	public async Task Post_WithLargeDimension_ReturnsWithEarlyFail_ItemDimensionExceeded()
	{
		var request = this.CreateSpecialRequest();
		var specialBox = request.Items!.FirstOrDefault(x => x.ID == "special_box_1")!;
		specialBox.Length = 61;
		specialBox.Width = 5;
		specialBox.Height = 5;

		var preset = presetOptions.Value.Presets["special"];
		var urlPath = routePath.Replace("{preset}", "special");

		await base.FitRequest_ValidateBasedOnParameters(
			urlPath,
			request,
			result =>
			{
				result.Data.ShouldHaveCount(preset.Bins!.Count);
				foreach (var binFitResult in result.Data)
				{
					binFitResult.Result.ShouldBe(BinFitResultStatus.EarlyFail_ItemDimensionExceeded);
				}
			}
		);
	}
	
	[Fact(DisplayName = $"POST {routePath}. With Report Fitted Items, Returns With Fitted Items")]
	public async Task Post_WithReportFittedItems_ReturnsFittedItems()
	{
		var request = this.CreateSpecialRequest(parameters => parameters.ReportFittedItems = true);

		var preset = presetOptions.Value.Presets["special"];
		var urlPath = routePath.Replace("{preset}", "special");

		await base.FitRequest_ValidateBasedOnParameters(
			urlPath,
			request,
			result =>
			{
				result.Data.ShouldHaveCount(preset.Bins!.Count);
			}
		);
	}

	[Fact(DisplayName = $"POST {routePath}. With Report Unfitted Items Returns With Unfitted Items")]
	public async Task Post_WithReportUnfittedItems_ReturnsUnfittedItems()
	{
		var request = this.CreateSpecialRequest(parameters => parameters.ReportUnfittedItems = true);
		
		var preset = presetOptions.Value.Presets["special"];
		var urlPath = routePath.Replace("{preset}", "special");

		await base.FitRequest_ValidateBasedOnParameters(
			urlPath,
			request,
			result =>
			{
				result.Data.ShouldHaveCount(preset.Bins!.Count);
			}
		);
	}

	[Fact(DisplayName = $"POST {routePath}. With Find Smallest Bin Only Returns With Smallest Bin Only")]
	public async Task Post_WithFindSmallestBinOnly_ReturnsWithSmallestBinOnly()
	{
		this.sampleRequest.Parameters!.FindSmallestBinOnly = true;

		var preset = presetOptions.Value.Presets[validPreset];
		var expectedBin = preset.Bins!
			.OrderBy(x => x.Length * x.Width * x.Height)
			.FirstOrDefault()!;
		var urlPath = routePath.Replace("{preset}", validPreset);

		await base.FitRequest_ValidateBasedOnParameters(
			urlPath,
			this.sampleRequest,
			result =>
			{
				foreach (var binFitResult in result.Data)
				{
					binFitResult.Bin.ID.ShouldBe(expectedBin.ID);
				}
			}
		);
	}
	
	private PresetFitRequest CreateSpecialRequest(
		Action<FitRequestParameters>? modifyParameters = null
	)
	{
		var request = new PresetFitRequest
		{
			Parameters = new()
			{
				FindSmallestBinOnly = false,
				ReportFittedItems = false,
				ReportUnfittedItems = false
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
