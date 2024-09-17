using Binacle.Net.Api.UIModule.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace Binacle.Net.Api.UIModule.Components.Features;

public partial class BinPackingForm : ComponentBase
{
	protected BinPackingViewModel Model { get; set; }

	[Inject]
	protected IHttpClientFactory HttpClientFactory { get; set; }

	[Inject]
	protected IJSRuntime JsRuntime { get; set; }

	[Inject]
	protected Services.ISampleDataService SampleDataService { get; set; }

	protected override void OnInitialized()
	{
		Model = SampleDataService.GetInitialSampleData();
	}

	protected async Task BinChangedAsync()
	{
		await JsRuntime.InvokeVoidAsync("binacle.binsChanged", Model.Bins);
	}

	protected void AddItem()
	{
		Model.Items.Add(new Item(0, 0, 0, 1));
	}

	protected void RemoveItem(Item item)
	{
		Model.Items.Remove(item);
	}

	protected void ClearAllItems()
	{
		Model.Items.Clear();
	}

	protected Task RandomizeItemsFromSamplesAsync()
	{
		var sampleData = SampleDataService.GetRandomSampleData();
		Model.Items = sampleData.Items;
		return Task.CompletedTask;
	}

	protected async Task RandomizeBinsFromSamplesAsync()
	{
		var sampleData = SampleDataService.GetRandomSampleData();
		Model.Bins = sampleData.Bins;
		await BinChangedAsync();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			await JsRuntime.InvokeVoidAsync("binacle.initialize", Model.Bins);
		}
	}

	protected async Task GetResultsAsync()
	{
		await JsRuntime.InvokeVoidAsync("binacle.loading");
		try
		{
			var request = new ApiModels.Requests.PackByCustomRequest
			{
				Bins = Model.Bins.Select(x => new ApiModels.Bin(x.ID, x)).ToList(),
				Items = Model.Items.Select(x => new ApiModels.Item(x.ID, x, x.Quantity)).ToList()
			};
			var client = HttpClientFactory.CreateClient("Self");
			var response = await client.PostAsJsonAsync("api/v2/pack/by-custom", request);
			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				var results = await response.Content.ReadFromJsonAsync<JsonObject>();
				await JsRuntime.InvokeVoidAsync("binacle.updateResults", results);
			}
			else
			{
				var results = await response.Content.ReadFromJsonAsync<JsonObject>();
				await JsRuntime.InvokeVoidAsync("binacle.invokeErrors", results);
			}
		}
		catch (Exception ex)
		{

		}

	}

}
