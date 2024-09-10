using Binacle.Net.Api.UIModule.Models;
using Binacle.Net.Lib.Packing.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Binacle.Net.Api.UIModule.Components.Features;

public partial class BinPacking : ComponentBase
{
	protected BinPackingViewModel Model { get; set; }

	protected Dictionary<string, PackingResult>? Results { get; set; }
	public string RawResult { get; private set; }
	[Inject]
	protected IHttpClientFactory HttpClientFactory { get; set; }

	[Inject]
	protected IJSRuntime JsRuntime { get; set; }

	[Inject]
	protected Services.ISampleDataService SampleDataService { get; set; }
	
	protected override void OnInitialized()
	{
		this.Model = this.SampleDataService.GetInitialSampleData();
	}

	protected async Task BinChangedAsync()
	{
		await this.JsRuntime.InvokeVoidAsync("containerChanged", this.Model.Bins.First());
	}

	protected void AddItem()
	{
		this.Model.Items.Add(new Item(0, 0, 0, 1));
	}

	protected void RemoveItem(Item item)
	{
		this.Model.Items.Remove(item);
	}

	protected void ClearAllItems()
	{
		this.Model.Items.Clear();
	}

	protected Task RandomizeItemsFromSamplesAsync()
	{
		var sampleData = this.SampleDataService.GetRandomSampleData();
		this.Model.Items = sampleData.Items;
		return Task.CompletedTask;
	}

	protected async Task RandomizeBinsFromSamplesAsync()
	{
		var sampleData = this.SampleDataService.GetRandomSampleData();
		this.Model.Bins = sampleData.Bins;
		await this.BinChangedAsync();
	}

	protected async Task GetResultsAsync()
	{
		var request = new ApiModels.Requests.PackByCustomRequest
		{
			Bins = this.Model.Bins.Select(x => new ApiModels.Bin(x)).ToList(),
			Items = this.Model.Items.Select(x => new ApiModels.Item(x, x.Quantity)).ToList()
		};
		var client = this.HttpClientFactory.CreateClient("Self");
		var response = await client.PostAsJsonAsync("api/v2/pack/by-custom", request);
		response.EnsureSuccessStatusCode();
		this.RawResult = await response.Content.ReadAsStringAsync();
		await this.JsRuntime.InvokeVoidAsync("updateResults", this.RawResult);
	}

}
