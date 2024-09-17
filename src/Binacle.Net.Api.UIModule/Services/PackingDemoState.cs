using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace Binacle.Net.Api.UIModule.Services;

internal class PackingDemoState
{
	private readonly IHttpClientFactory httpClientFactory;
	private readonly IJSRuntime jsRuntime;

	public PackingDemoState(
		IHttpClientFactory httpClientFactory,
		IJSRuntime jsRuntime
		)
	{
		this.httpClientFactory = httpClientFactory;
		this.jsRuntime = jsRuntime;
		this.Model = new ViewModels.BinPackingViewModel();
		this.Results = new List<Models.PackingResult>();
	}
	public ViewModels.BinPackingViewModel Model { get; set; }

	public async Task InitializeDomAsync()
	{
		await this.jsRuntime.InvokeVoidAsync("binacle.initialize", this.Model.Bins.FirstOrDefault());
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

				await this.TriggerResultsChangedAsync(results?.Data);
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

	public List<Models.PackingResult>? Results { get; private set; }
	public EventCallback ResultsChanged { get; set; }
	private async Task TriggerResultsChangedAsync(List<Models.PackingResult>? results)
	{
		this.Results = results ?? new List<Models.PackingResult>();
		await this.jsRuntime.InvokeVoidAsync("binacle.updateResult", this.Results.FirstOrDefault());
		await this.ResultsChanged.InvokeAsync(this.Results);
	}

	public EventCallback BinsChanged { get; set; }
	public async Task TriggerBinsChangedAsync()
	{
		await this.jsRuntime.InvokeVoidAsync("binacle.binChanged", this.Model.Bins?.FirstOrDefault());
		await this.BinsChanged.InvokeAsync(this.Model.Bins);
		await this.TriggerResultsChangedAsync(null);

	}
	public EventCallback ItemsChanged { get; set; }
	private async Task TriggerItemsChangedAsync()
	{
		await this.ItemsChanged.InvokeAsync(this.Model.Items);
	}

	internal async Task SetResultAsync(Models.PackingResult result)
	{
		await this.jsRuntime.InvokeVoidAsync("binacle.loading");
		var bin = this.Model.Bins.FirstOrDefault(x => x.ID == result.Bin.ID);
		await this.jsRuntime.InvokeVoidAsync("binacle.binChanged", bin);
		await this.jsRuntime.InvokeVoidAsync("binacle.updateResult", result);
	}

	public async Task ChangeBinsAsync(List<ViewModels.Bin> bins)
	{
		this.Model.Bins = bins;
		await this.TriggerBinsChangedAsync();
	}

	public async Task ChangeItemsAsync(List<ViewModels.Item> items)
	{
		this.Model.Items = items;
		await this.TriggerItemsChangedAsync();
	}

	public async Task AddItemAsync(ViewModels.Item item)
	{
		this.Model.Items.Add(item);
		await this.TriggerItemsChangedAsync();
	}

	public async Task RemoveItemAsync(ViewModels.Item item)
	{
		this.Model.Items.Remove(item);
		await this.TriggerItemsChangedAsync();
	}

	public async Task ClearAllItemsAsync()
	{
		this.Model.Items.Clear();
		await this.TriggerItemsChangedAsync();
	}

	public async Task AddBinAsync(ViewModels.Bin bin)
	{
		this.Model.Bins.Add(bin);
		await this.TriggerBinsChangedAsync();
	}

	public async Task RemoveBinAsync(ViewModels.Bin item)
	{
		this.Model.Bins.Remove(item);
		await this.TriggerBinsChangedAsync();
	}

	public async Task ClearAllBinsAsync()
	{
		this.Model.Bins.Clear();
		await this.TriggerBinsChangedAsync();
	}
}
