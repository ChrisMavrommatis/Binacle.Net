using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text.Json.Nodes;
using Binacle.Net.Api.UIModule.Models;
using System.Diagnostics;

namespace Binacle.Net.Api.UIModule.Services;

internal class PackingDemoState
{
	private readonly IHttpClientFactory httpClientFactory;
	private readonly IJSRuntime jsRuntime;
	private readonly ISampleDataService sampleDataService;

	public PackingDemoState(
		IHttpClientFactory httpClientFactory,
		IJSRuntime jsRuntime,
		Services.ISampleDataService sampleDataService
		)
	{
		this.httpClientFactory = httpClientFactory;
		this.jsRuntime = jsRuntime;
		this.sampleDataService = sampleDataService;
		this.Results = new List<Models.PackingResult>();
	}
	public ViewModels.BinPackingViewModel Model { get; set; }

	public List<Models.PackingResult>? Results { get; private set; }
	public EventCallback ResultsChanged { get; set; }

	private async Task ResultsChangedAsync(ApiModels.Responses.PackByCustomResponse response)
	{
		this.Results = response.Data;
		await this.jsRuntime.InvokeVoidAsync("binacle.updateResults", response);
		await this.ResultsChanged.InvokeAsync(this.Results);
	}

	public async Task GetResultsAsync()
	{
		await this.jsRuntime.InvokeVoidAsync("binacle.loading");
		try
		{
			var request = new ApiModels.Requests.PackByCustomRequest
			{
				Bins = Model.Bins.Select(x => new Models.Bin(x.ID, x)).ToList(),
				Items = Model.Items.Select(x => new Models.Item(x.ID, x, x.Quantity)).ToList()
			};
			var client = this.httpClientFactory.CreateClient("Self");
			var response = await client.PostAsJsonAsync("api/v2/pack/by-custom", request);
			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				var results = await response.Content.ReadFromJsonAsync<ApiModels.Responses.PackByCustomResponse>();

				await this.ResultsChangedAsync(results);
			}
			else
			{
				var results = await response.Content.ReadFromJsonAsync<JsonObject>();
				await this.jsRuntime.InvokeVoidAsync("binacle.invokeErrors", results);
			}
		}
		catch (Exception ex)
		{
			await this.jsRuntime.InvokeVoidAsync("binacle.invokeErrors", ex.Message);
		}

	}

	public EventCallback BinsChanged { get; set; }
	public async Task BinsChangedAsync()
	{
		await this.jsRuntime.InvokeVoidAsync("binacle.binsChanged", this.Model.Bins);
		await this.BinsChanged.InvokeAsync(this.Model.Bins);
	}

	public Task RandomizeItemsFromSamplesAsync()
	{
		var sampleData = this.sampleDataService.GetRandomSampleData();
		this.Model.Items = sampleData.Items;
		return Task.CompletedTask;
	}

	public async Task RandomizeBinsFromSamplesAsync()
	{
		var sampleData = this.sampleDataService.GetRandomSampleData();
		this.Model.Bins = sampleData.Bins;
		await this.BinsChangedAsync();
	}

	public async Task InitializeDomAsync()
	{
		await this.jsRuntime.InvokeVoidAsync("binacle.initialize", this.Model.Bins);
	}

	internal void InitializeModelWithSampleData()
	{
		this.Model = sampleDataService.GetInitialSampleData();
	}
}
