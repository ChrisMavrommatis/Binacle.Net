using Binacle.Net.Api.UIModule.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Binacle.Net.Api.UIModule.Components.Features;

internal partial class BinPacking : ComponentBase
{
	protected BinPackingViewModel Model { get; set; }

	internal ApiModels.Responses.PackByCustomResponse? PackResponse { get; set; }
	internal ApiModels.Responses.ErrorResponse? ErrorResponse { get; private set; }


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
		try
		{
			var request = new ApiModels.Requests.PackByCustomRequest
			{
				Bins = this.Model.Bins.Select(x => new ApiModels.Bin(x)).ToList(),
				Items = this.Model.Items.Select(x => new ApiModels.Item(x, x.Quantity)).ToList()
			};
			var client = this.HttpClientFactory.CreateClient("Self");
			var response = await client.PostAsJsonAsync("api/v2/pack/by-custom", request);
			if(response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				this.PackResponse = await response.Content.ReadFromJsonAsync<ApiModels.Responses.PackByCustomResponse>();
				await this.JsRuntime.InvokeVoidAsync("updateResults", this.PackResponse);
			}
			else
			{
				this.ErrorResponse = await response.Content.ReadFromJsonAsync<ApiModels.Responses.ErrorResponse>();
				await this.JsRuntime.InvokeVoidAsync("invokeErrors", this.PackResponse);
			}
		} 
		catch(Exception ex)
		{

		}
		
	}

}
