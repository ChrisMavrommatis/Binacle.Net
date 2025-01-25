using System.Net;
using System.Net.Http.Json;
using Binacle.Net.Api.UIModule.ApiModels.Requests;
using Binacle.Net.Api.UIModule.ApiModels.Responses;
using Binacle.Net.Api.UIModule.Components.Features;
using Binacle.Net.Api.UIModule.Models;
using Binacle.Net.Api.UIModule.Services;
using Binacle.Net.Api.UIModule.ViewModels;
using Microsoft.AspNetCore.Components;
using Bin = Binacle.Net.Api.UIModule.ViewModels.Bin;
using Item = Binacle.Net.Api.UIModule.ViewModels.Item;

namespace Binacle.Net.Api.UIModule.Components.Pages;

public partial class PackingDemo : ComponentBase
{
	[Inject] internal ISampleDataService? SampleDataService { get; set; }

	[Inject] protected IHttpClientFactory? HttpClientFactory { get; set; }

	[Inject] protected MessagingService? MessagingService { get; set; }


	private List<string> errors = new();
	private List<PackingResult>? results;
	private PackingResult? selectedResult;

	internal BinPackingViewModel Model { get; set; } = new()
	{
		Algorithm = Algorithm.FirstFitDecreasing,
		Bins = new List<Bin>(),
		Items = new List<Item>()
	};

	protected override void OnInitialized()
	{
		var sampleData = this.SampleDataService!.GetInitialSampleData();
		this.Model = sampleData;
	}

	protected override async Task OnParametersSetAsync()
	{
		var bin = this.Model.Bins.FirstOrDefault()!;
		this.selectedResult = new()
		{
			Bin = new UIModule.Models.Bin()
			{
				ID = bin.ID,
				Length = bin.Length,
				Width = bin.Width,
				Height = bin.Height,
			}
		};
		await base.OnParametersSetAsync();
	}

	internal void RemoveBin(Bin bin)
	{
		this.Model.Bins.Remove(bin);
	}

	internal void AddBin()
	{
		var bin = new Bin(0, 0, 0);
		this.Model.Bins.Add(bin);
	}

	internal void ClearAllBins()
	{
		this.Model.Bins.Clear();
	}

	internal void RandomizeBinsFromSamples()
	{
		var sampleData = this.SampleDataService!.GetRandomSampleData();
		this.Model.Bins = sampleData.Bins;
	}

	internal void RemoveItem(Item item)
	{
		this.Model.Items.Remove(item);
	}

	internal void AddItem()
	{
		var item = new Item(0, 0, 0, 1);
		this.Model.Items.Add(item);
	}

	internal void ClearAllItems()
	{
		this.Model.Items.Clear();
	}

	internal void RandomizeItemsFromSamples()
	{
		var sampleData = this.SampleDataService!.GetRandomSampleData();
		this.Model.Items = sampleData.Items;
	}

	internal async Task GetResults()
	{
		await this.MessagingService!.TriggerAsync<AsyncCallback<(UIModule.Models.Bin?, List<UIModule.Models.PackedItem>?)>>(
			"UpdateScene",
			async () =>
			{
				this.errors.Clear();
				try
				{
					var request = new PackByCustomRequest
					{
						Parameters = new PackRequestParameters()
						{
							Algorithm = this.Model.Algorithm switch
							{
								Algorithm.FirstFitDecreasing => ApiModels.Algorithm.FFD,
								Algorithm.BestFitDecreasing => ApiModels.Algorithm.BFD,
								Algorithm.WorstFitDecreasing => ApiModels.Algorithm.WFD,
								_ => throw new ArgumentOutOfRangeException()
							}
						},
						Bins = this.Model.Bins.Select(x => new UIModule.Models.Bin(x.ID, x)).ToList(),
						Items = this.Model.Items.Select(x => new UIModule.Models.Item(x.ID, x, x.Quantity)).ToList()
					};
					var client = this.HttpClientFactory!.CreateClient("BinacleApi");
					var response = await client.PostAsJsonAsync("api/v3/pack/by-custom", request);
					if (response.StatusCode != HttpStatusCode.OK)
					{
						throw new ApplicationException($"Error: {response.StatusCode}");
					}

					var packResponse = await response.Content.ReadFromJsonAsync<PackByCustomResponse>();
					if (packResponse is null || packResponse.Data is null || packResponse.Data.Count < 1)
					{
						throw new ApplicationException("No results found");
					}

					this.results = packResponse.Data ?? new List<PackingResult>();

					var result = this.results!.FirstOrDefault()!;

					if (result.Bin is null)
					{
						throw new InvalidOperationException("Selected result has no bin");
					}

					this.selectedResult = result;
					return (result.Bin, result.PackedItems);
				}
				catch (Exception ex)
				{
					this.errors.Add(ex.Message);
					return (null, null);
				}
			});
	}
}
