using Binacle.Net.Configuration.Models;
using Binacle.Net.IntegrationTests.v2.Abstractions;
using Binacle.Net.v2.Models;
using Binacle.Net.v2.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.IntegrationTests.v2;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class PackByPresetBehavior :  BehaviourTestsBase
{
	private readonly IOptions<BinPresetOptions> presetOptions;
	private readonly PresetPackRequest sampleRequest = new()
	{
		Parameters = new()
		{
			NeverReportUnpackedItems = false,
			ReportPackedItemsOnlyWhenFullyPacked = false,
			OptInToEarlyFails = false,
			StopAtSmallestBin = false,
		},
		Items = new()
		{
			new (){ ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
			new (){ ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
			new (){ ID = "box_3", Quantity = 1, Length = 12, Width = 10, Height = 15 },
		}
	};

	private const string routePath = "/api/v2/pack/by-preset/{preset}";
	private const string validPreset = "rectangular-cuboids";

	public PackByPresetBehavior(BinacleApi sut) : base(sut)
	{
		this.presetOptions = this.Sut.Services.GetRequiredService<IOptions<BinPresetOptions>>();
	}
	
	#region Response Statuses

	[Fact(DisplayName = $"POST {routePath}. With Existing Preset And Valid Request Returns 200 OK")]
	public async Task Post_WithExistingPresetAndValidRequest_Returns200Ok()
	{
		this.sampleRequest!.Items!.FirstOrDefault(x => x.ID == "box_2")!.Length = 0;
		var urlPath = routePath.Replace("{preset}", validPreset);
		await base.Request_Returns_400BadRequest(urlPath, this.sampleRequest);
	}
	
	[Fact(DisplayName = $"POST {routePath}. With Non Existing Preset Returns 404 NotFound")]
	public async Task Post_WithNonExistingPreset_Returns_404NotFound()
	{
		var urlPath = routePath.Replace("{preset}", "non-existing-preset");
		await base.Request_Returns_404NotFound(urlPath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Zero Dimensions Returns 400 BadRequest")]
	public async Task Post_WithZeroDimensions_Returns400BadRequest()
	{
		this.sampleRequest!.Items!.FirstOrDefault(x => x.ID == "box_2")!.Length = 0;
		var urlPath = routePath.Replace("{preset}", validPreset);
		await base.Request_Returns_400BadRequest(urlPath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Same Id Returns 400 BadRequest")]
	public async Task Post_WithSameId_Returns400BadRequest()
	{
		var urlPath = routePath.Replace("{preset}", validPreset);
		foreach (var bin in this.sampleRequest!.Items!)
		{
			bin.ID = "box_1";
		}
		await base.Request_Returns_400BadRequest(urlPath, this.sampleRequest);

	}
	#endregion Response Statues

	#region Response Data

	[Fact(DisplayName = $"POST {routePath}. With Default Parameters, Reports All Items")]
	public async Task Post_WithDefaultParameters_ReportsAllItems()
	{
		var request = this.CreateSpecialRequest();

		var preset = presetOptions.Value.Presets["special"];
		var urlPath = routePath.Replace("{preset}", "special");

		await base.PackRequest_ValidateBasedOnParameters(
			urlPath,
			request,
			(result) =>
			{
				result.Data.ShouldHaveCount(preset.Bins!.Count);
			}
		);
	}
	
	[Fact(DisplayName = $"POST {routePath}. With Report Packed Items Only When Fully Packed, Does Not Report Them On Partial Pack")]
	public async Task Post_WithReportPackedItemsOnlyWhenFullyPacked_DoesNotReportThemOnPartialPack()
	{
		var request = this.CreateSpecialRequest(parameters => parameters.ReportPackedItemsOnlyWhenFullyPacked = true);

		var preset = presetOptions.Value.Presets["special"];
		var urlPath = routePath.Replace("{preset}", "special");
		
		await base.PackRequest_ValidateBasedOnParameters(
			urlPath,
			request,
			(result) =>
			{
				result.Data.ShouldHaveCount(preset.Bins!.Count);
			}
		);
	}
	
	[Fact(DisplayName = $"POST {routePath}. With Never Report Unpacked Items, Does Not Report Them On Partial Pack")]
	public async Task Post_WithNeverReportUnpackedItems_DoesNotReportThemOnPartialPack()
	{
		var request = this.CreateSpecialRequest(parameters => parameters.NeverReportUnpackedItems = true);

		var preset = presetOptions.Value.Presets["special"];
		var urlPath = routePath.Replace("{preset}", "special");

		await base.PackRequest_ValidateBasedOnParameters(
			urlPath,
			request,
			(result) =>
			{
				result.Data.ShouldHaveCount(preset.Bins!.Count);
			}
		);
	}

	[Fact(DisplayName = $"POST {routePath}. With Opt In To Early Fails And Large Volume, Returns Early Fail Container Volume Exceeded")]
	public async Task Post_WithEarlyFailOptInAndLargeVolume_ReturnsWithEarlyFail_ContainerVolumeExceeded()
	{
		var request = this.CreateSpecialRequest(parameters => parameters.OptInToEarlyFails = true);
		request.Items!.FirstOrDefault(x => x.ID == "special_box_1")!.Quantity = 3;

		var preset = presetOptions.Value.Presets["special"];
		var urlPath = routePath.Replace("{preset}", "special");

		await base.PackRequest_ValidateBasedOnParameters(
			urlPath,
			request,
			(result) =>
			{
				result.Data.ShouldHaveCount(preset.Bins!.Count);
				foreach (var binFitResult in result.Data)
				{
					binFitResult.Result.ShouldBe(BinPackResultStatus.EarlyFail_ContainerVolumeExceeded);
				}
			}
		);
	}
	
	[Fact(DisplayName = $"POST {routePath}. With Early Fail Opt In And Large Dimension, Returns Early Fail Container Dimension Exceeded")]
	public async Task Post_WithEarlyFailOptInAndLargeDimension_ReturnsWithEarlyFail_ContainerDimensionExceeded()
	{
		var request = this.CreateSpecialRequest(parameters => parameters.OptInToEarlyFails = true);
		var specialBox = request.Items!.FirstOrDefault(x => x.ID == "special_box_1")!;
		specialBox.Length = 61;
		specialBox.Width = 5;
		specialBox.Height = 5;

		var preset = presetOptions.Value.Presets["special"];
		var urlPath = routePath.Replace("{preset}", "special");

		await base.PackRequest_ValidateBasedOnParameters(
			urlPath,
			request,
			(result) =>
			{
				result.Data.ShouldHaveCount(preset.Bins!.Count);
				foreach (var binFitResult in result.Data)
				{
					binFitResult.Result.ShouldBe(BinPackResultStatus.EarlyFail_ContainerDimensionExceeded);
				}
			}
		);
	}
	
	[Fact(DisplayName = $"POST {routePath}. With Stop At Smallest Bin, Returns With Smallest Bin Only")]
	public async Task Post_WithStopAtSmallestBin_ReturnsWithSmallestBinOnly()
	{
		this.sampleRequest.Parameters!.StopAtSmallestBin = true;
		var preset = presetOptions.Value.Presets[validPreset];

		var expectedBin = preset.Bins!
			.OrderBy(x => x.Length * x.Width * x.Height)
			.FirstOrDefault()!;
		
		var urlPath = routePath.Replace("{preset}", validPreset);

		await base.PackRequest_ValidateBasedOnParameters(
			urlPath,
			this.sampleRequest,
			(result) =>
			{
				foreach (var binFitResult in result.Data)
				{
					binFitResult.Bin.ID.ShouldBe(expectedBin.ID);
				}
			}
		);
	}
	
	private PresetPackRequest CreateSpecialRequest(
		Action<PackRequestParameters>? configure = null
		)
	{
		var request = new PresetPackRequest
		{
			Parameters = new()
			{
				NeverReportUnpackedItems = false,
				ReportPackedItemsOnlyWhenFullyPacked = false,
				OptInToEarlyFails = false,
				StopAtSmallestBin = false,
			},
			Items = new()
			{
				new() { ID = "special_box_1", Quantity = 1, Length = 8, Width = 40, Height = 60 },
				new() { ID = "box_1", Quantity = 1, Length = 5, Width = 5, Height = 5 },
			}
		};

		configure?.Invoke(request.Parameters);

		return request;
	}

	#endregion Response Data
}
