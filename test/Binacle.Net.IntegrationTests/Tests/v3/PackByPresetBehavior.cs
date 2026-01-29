using Binacle.Net.Configuration;
using Binacle.Net.Models;
using Binacle.Net.v3.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.IntegrationTests.v3;

[Trait("Behavioral Tests", "Ensures operations behave as expected")]
public class PackByPresetBehavior:  Abstractions.BehaviourTestsBase
{
	private readonly IOptions<BinPresetOptions> presetOptions;
	private readonly PackByPresetRequest sampleRequest = new()
	{
		Parameters = new()
		{
			Algorithm = Algorithm.FFD,
			IncludeViPaqData = false
		},
		Items = new()
		{
			new (){ ID = "box_1", Quantity = 2, Length = 2, Width = 5, Height = 10 },
			new (){ ID = "box_2", Quantity = 1, Length = 12, Width = 15, Height = 10 },
			new (){ ID = "box_3", Quantity = 1, Length = 12, Width = 10, Height = 15 },
		}
	};

	private const string routePath = "/api/v3/pack/by-preset/{preset}";

	public PackByPresetBehavior(BinacleApi sut) : base(sut)
	{
		this.presetOptions = this.Sut.Services.GetRequiredService<IOptions<BinPresetOptions>>();
	}
	
	#region Response Statuses
	[Fact(DisplayName = $"POST {routePath}. With Existing Preset And Valid Request Returns 200 OK")]
	public async Task Post_WithExistingPresetAndValidRequest_Returns200Ok()
	{
		var urlPath = routePath.Replace("{preset}", PresetKeys.CustomProblems);
		await base.Request_Returns_200Ok(urlPath, this.sampleRequest);
	}
	
	[Fact(DisplayName = $"POST {routePath}. With Non Existing Preset Returns 404 NotFound")]
	public async Task Post_WithNonExistingPreset_Returns_404NotFound()
	{
		var urlPath = routePath.Replace("{preset}", "non-existing-preset");
		await base.Request_Returns_404NotFound(urlPath, this.sampleRequest);
	}
	
	[Fact(DisplayName = $"POST {routePath}. With Zero Dimension On Item, Returns 422 UnprocessableContent")]
	public async Task Post_WithZeroDimensionOnItem_Returns422UnprocessableContent()
	{
		var urlPath = routePath.Replace("{preset}", PresetKeys.CustomProblems);
		
		this.sampleRequest.Items!.FirstOrDefault(x => x.ID == "box_2")!.Length = 0;
		await base.Request_Returns_422UnprocessableContent(urlPath, this.sampleRequest);
	}

	[Fact(DisplayName = $"POST {routePath}. With Same Id On Items, Returns 422 UnprocessableContent")]
	public async Task Post_WithSameIdOnItems_Returns422UnprocessableContent()
	{
		foreach (var bin in this.sampleRequest.Items!)
		{
			bin.ID = "box_1";
		}
		var urlPath = routePath.Replace("{preset}", PresetKeys.CustomProblems);
		await base.Request_Returns_422UnprocessableContent(urlPath, this.sampleRequest);
	}

	#endregion Response Statuses
	
	
	#region Response Data
	
	[Fact(DisplayName = $"POST {routePath}. With Default Parameters, Reports All Items")]
	public async Task Post_WithDefaultParameters_ReportsAllItems()
	{
		var request = this.CreateSpecialRequest();
		var preset = presetOptions.Value.Presets[PresetKeys.SpecialSet];
		var urlPath = routePath.Replace("{preset}", PresetKeys.SpecialSet);
		await base.PackRequest_ValidateBasedOnParameters(
			urlPath,
			request,
			result =>
			{
				result.Data.ShouldHaveCount(preset.Bins!.Count);
			}
		);
	}
	
	
	[Fact(DisplayName = $"POST {routePath}. With ViPaq Data, Returns ViPaq Data")]
	public async Task Post_WithViPaqData_ReturnsViPaqData()
	{
		var request = this.CreateSpecialRequest(parameters => parameters.IncludeViPaqData = true);
		var preset = presetOptions.Value.Presets[PresetKeys.SpecialSet];
		var urlPath = routePath.Replace("{preset}", PresetKeys.SpecialSet);
		await base.PackRequest_ValidateBasedOnParameters(
			urlPath,
			request,
			result =>
			{
				foreach (var packResult in result.Data)
				{
					packResult.ViPaqData.ShouldNotBeNullOrEmpty();
				}
			}
		);
	}
	
	private Binacle.Net.v3.Contracts.PackByPresetRequest CreateSpecialRequest(
		Action<Binacle.Net.v3.Contracts.PackRequestParameters>? modifyParameters = null
	)
	{
		var request = new Binacle.Net.v3.Contracts.PackByPresetRequest
		{
			Parameters = new()
			{
				Algorithm = Algorithm.FFD,
				IncludeViPaqData = false
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
