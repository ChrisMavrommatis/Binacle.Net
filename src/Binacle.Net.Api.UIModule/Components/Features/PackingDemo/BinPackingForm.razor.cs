// using Binacle.Net.Api.UIModule.Services;
// using Microsoft.AspNetCore.Components;
//
// namespace Binacle.Net.Api.UIModule.Components.Features;
//
// public partial class BinPackingForm : ComponentBase
// {
// 	[Inject]
// 	internal Services.PackingDemoState? State { get; set; }
//
// 	[Inject]
// 	internal ISampleDataService? SampleDataService { get; set; }
//
// 	protected override Task OnInitializedAsync()
// 	{
// 		var sampleData = this.SampleDataService!.GetInitialSampleData();
// 		this.State!.Model = sampleData;
// 		return Task.CompletedTask;
// 	}
//
// 	public async Task RandomizeItemsFromSamplesAsync()
// 	{
// 		var sampleData = this.SampleDataService!.GetRandomSampleData();
// 		await this.State!.ChangeItemsAsync(sampleData.Items);
// 	}
//
// 	public async Task RandomizeBinsFromSamplesAsync()
// 	{
// 		var sampleData = this.SampleDataService!.GetRandomSampleData();
// 		await this.State!.ChangeBinsAsync(sampleData.Bins);
// 	}
//
// 	internal async Task AddItemAsync()
// 	{
// 		await this.State!.AddItemAsync(new ViewModels.Item(0, 0, 0, 1));
// 	}
//
// 	internal async Task RemoveItemAsync(ViewModels.Item item)
// 	{
// 		await this.State!.RemoveItemAsync(item);
// 	}
//
// 	internal async Task ClearAllItemsAsync()
// 	{
// 		await this.State!.ClearAllItemsAsync();
// 	}
//
// 	internal async Task AddBinAsync()
// 	{
// 		await this.State!.AddBinAsync(new ViewModels.Bin(0, 0, 0));
// 	}
//
// 	internal async Task RemoveBinAsync(ViewModels.Bin bin)
// 	{
// 		await this.State!.RemoveBinAsync(bin);
// 	}
//
// 	internal async Task ClearAllBinsAsync()
// 	{
// 		await this.State!.ClearAllBinsAsync();
// 	}
//
// 	internal async Task BinsChangedAsync()
// 	{
// 		await this.State!.TriggerBinsChangedAsync();
// 	}
//
// 	protected override async Task OnAfterRenderAsync(bool firstRender)
// 	{
// 		if (firstRender)
// 		{
// 			await this.State!.InitializeDomAsync();
// 		}
// 	}
// }
