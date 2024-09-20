using Binacle.Net.Api.UIModule.Models;
using Binacle.Net.Lib.Abstractions.Models;
using Microsoft.JSInterop;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

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
		this.results = new List<Models.PackingResult>();
	}
	public ViewModels.BinPackingViewModel Model { get; set; }

	public ValueTask InitializeDomAsync()
		=> this.jsRuntime.InvokeVoidAsync("binacle.initialize", this.Model.Bins.FirstOrDefault());

	public ValueTask UpdateLoadingStart()
		=> this.jsRuntime.InvokeVoidAsync("binacle.loadingStart");

	public ValueTask UpdateLoadingEnd()
		=> this.jsRuntime.InvokeVoidAsync("binacle.loadingEnd");

	public ValueTask InvokeErrors(Exception ex)
		=> this.jsRuntime.InvokeVoidAsync("binacle.invokeErrors", ex.Message);

	public ValueTask RedrawSceneAsync<TBin>(TBin bin, List<PackedItem>? items)
		where TBin : IWithID, IWithReadOnlyDimensions
		=> this.jsRuntime.InvokeVoidAsync("binacle.redrawScene", bin, items);

	public ValueTask AddItemToScene<TBin>(TBin bin, PackedItem item, int index)
		=> this.jsRuntime.InvokeVoidAsync("binacle.addItemToScene", bin, item, index);

	public ValueTask RemoveItemFromScene(int index)
		=> this.jsRuntime.InvokeVoidAsync("binacle.removeItemFromScene", index);

	public async Task GetResultsAsync()
	{
		await this.UpdateLoadingStart();
		try
		{
			var request = new ApiModels.Requests.PackByCustomRequest
			{
				Bins = Model.Bins.Select(x => new Models.Bin(x.ID, x)).ToList(),
				Items = Model.Items.Select(x => new Models.Item(x.ID, x, x.Quantity)).ToList()
			};
			var client = this.httpClientFactory.CreateClient("Self");
			var response = await client.PostAsJsonAsync("api/v2/pack/by-custom", request);
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
			{
				throw new ApplicationException($"Error: {response.StatusCode}");
			}

			var results = await response.Content.ReadFromJsonAsync<ApiModels.Responses.PackByCustomResponse>();
			if (results is null || results.Data is null || results.Data.Count < 1)
			{
				throw new ApplicationException("No results found");
			}
			this.results = results.Data ?? new List<Models.PackingResult>();

			var result = this.Results!.FirstOrDefault()!;
			await this.SelectResultAsync(result);

			await this.OnResultsChanged.InvokeAsync(this.Results!);
			await this.OnSelectResult.InvokeAsync(result);
		}
		catch (Exception ex)
		{
			// TODO
			await this.InvokeErrors(ex);
		}
		await this.UpdateLoadingEnd();

	}

	private List<Models.PackingResult>? results;
	public ReadOnlyCollection<Models.PackingResult>? Results => this.results?.AsReadOnly();

	public AsyncEvent<ReadOnlyCollection<Models.PackingResult>> OnResultsChanged { get; set; } = new();


	public AsyncEvent<Models.PackingResult> OnSelectResult { get; set; } = new();

	public Models.PackingResult? SelectedResult { get; private set; }
	public async Task SelectResultAsync(Models.PackingResult result)
	{
		var existingResult = this.Results?.FirstOrDefault(x => x.Bin.ID == result.Bin.ID);
		if (existingResult is null)
		{
			throw new ApplicationException("Could not find selected result");

		}
		this.SelectedResult = result;

		await this.OnSelectResult.InvokeAsync(result);

		await this.RedrawSceneAsync(result.Bin, result.PackedItems);

	}

	public AsyncEvent<List<ViewModels.Bin>> OnBinsChanged { get; set; } = new();
	public async Task TriggerBinsChangedAsync()
	{
		await this.OnBinsChanged.InvokeAsync(this.Model.Bins);
		if(this.SelectedResult is null)
		{
			await this.RedrawSceneAsync(this.Model.Bins.FirstOrDefault()!, null);
		}

	}
	public AsyncEvent<List<ViewModels.Item>> OnItemsChanged { get; set; } = new();
	private async Task TriggerItemsChangedAsync()
	{
		await this.OnItemsChanged.InvokeAsync(this.Model.Items);
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

	public bool IsSelected(Models.PackingResult result)
	{
		return this.SelectedResult == result;
	}
}
