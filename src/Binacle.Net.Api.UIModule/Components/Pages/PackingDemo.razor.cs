using System.Net;
using System.Net.Http.Json;
using Binacle.Net.Api.UIModule.Models;
using Binacle.Net.Api.UIModule.Services;
using Binacle.Net.Api.UIModule.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Bin = Binacle.Net.Api.UIModule.ViewModels.Bin;
using Item = Binacle.Net.Api.UIModule.ViewModels.Item;

namespace Binacle.Net.Api.UIModule.Components.Pages;

public partial class PackingDemo : AppletComponentBase
{
	protected override string Ref => "PackingDemo";
	
	[Inject] 
	internal ISampleDataService? SampleDataService { get; set; }

	[Inject] 
	protected IHttpClientFactory? HttpClientFactory { get; set; }

	[Inject] 
	internal MessagingService? MessagingService { get; set; }

	private Errors errors = new();
	private List<PackingResult>? results;
	private PackingResult? selectedResult;

	internal PackingDemoViewModel Model { get; set; } = new()
	{
		Algorithm = Algorithm.FirstFitDecreasing,
		Bins = new List<Bin>(),
		Items = new List<Item>()
	};

	protected override void OnInitialized()
	{
		base.OnInitialized();
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

	private void RemoveBin(Bin bin)
	{
		this.Model.Bins.Remove(bin);
	}

	private void AddBin()
	{
		var bin = new Bin(0, 0, 0);
		this.Model.Bins.Add(bin);
	}

	private void ClearAllBins()
	{
		this.Model.Bins.Clear();
	}

	private void RandomizeBinsFromSamples()
	{
		var sampleData = this.SampleDataService!.GetRandomSampleData();
		this.Model.Bins = sampleData.Bins;
	}

	private void RemoveItem(Item item)
	{
		this.Model.Items.Remove(item);
	}

	private void AddItem()
	{
		var item = new Item(0, 0, 0, 1);
		this.Model.Items.Add(item);
	}

	private void ClearAllItems()
	{
		this.Model.Items.Clear();
	}

	private void RandomizeItemsFromSamples()
	{
		var sampleData = this.SampleDataService!.GetRandomSampleData();
		this.Model.Items = sampleData.Items;
	}

	private async Task GetResults(EditContext editContext)
	{
		var messages = editContext.GetValidationMessages().ToList();
		if (messages.Count > 0)
		{
			this.errors.AddRange(messages);
			return;
		}
		await this.MessagingService!.TriggerAsync<AsyncCallback<(UIModule.Models.Bin?, List<UIModule.Models.PackedItem>?)>>(
			"UpdateScene",
			async () =>
			{
				try
				{
					var request = new ApiModels.Requests.PackByCustomRequest
					{
						Parameters = new ApiModels.Requests.PackRequestParameters()
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
						this.errors.Add($"Error: {response.StatusCode}.");
						var errorResponse = await response.Content.ReadFromJsonAsync<ApiModels.Responses.ErrorResponse>();
						if (errorResponse is null)
						{
							this.errors.Add($"Could not read the response");
							return (null, null);
						}

						if (!string.IsNullOrEmpty(errorResponse.Message))
						{
							this.errors.Add(errorResponse.Message);
						}

						if (errorResponse.Data is null)
						{
							return (null, null);
						}
						foreach (var error in errorResponse.Data)
						{
							if (!string.IsNullOrEmpty(error.Field))
							{
								this.errors.Add($"{error.Field} - {error.FieldError}");
							}

							if (!string.IsNullOrEmpty(error.Parameter))
							{
								this.errors.Add($"{error.Parameter} - {error.Message}");
							}
						}

						return (null, null);
					}

					var packResponse = await response.Content.ReadFromJsonAsync<ApiModels.Responses.PackByCustomResponse>();
					if (packResponse is null || packResponse.Data is null || packResponse.Data.Count < 1)
					{
						this.errors.Add($"No results found");
						return (null, null);
					}

					this.results = packResponse.Data ?? new List<PackingResult>();

					var result = this.results!.FirstOrDefault()!;

					if (result.Bin is null)
					{
						this.errors.Add($"Selected result has no bin");
						return (null, null);
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
	
	private async Task SelectResult(UIModule.Models.PackingResult result)
	{
		await this.MessagingService!.TriggerAsync<AsyncCallback<(UIModule.Models.Bin?, List<UIModule.Models.PackedItem>?)>>(
			"UpdateScene",
			() =>
			{
				try
				{
					if(result.Bin is null)
					{
						throw new InvalidOperationException("Selected result has no bin");
					}

					var existingResult = this.results?.FirstOrDefault(x => x.Bin!.ID == result.Bin.ID);
					if (existingResult is null)
					{
						throw new InvalidOperationException("Could not find selected result");
					}
					
					this.selectedResult = result;
					var returnedResult = (this.selectedResult.Bin, this.selectedResult.PackedItems);
					return Task.FromResult(returnedResult)!;
				}
				catch (Exception ex)
				{
					this.errors.Add(ex.Message);
					var returnedResult = (default(UIModule.Models.Bin?), default(List<UIModule.Models.PackedItem>?));
					return Task.FromResult(returnedResult)!;
				}
			});
	}
	
	private string GetColorClass(UIModule.Models.PackingResult result)
	{
		var baseColor = result.Result switch
		{
			UIModule.Models.PackResultType.FullyPacked => "green",
			UIModule.Models.PackResultType.PartiallyPacked => "orange",
			_ => "red"
		};
		return baseColor;
	}

	private bool IsSelected(UIModule.Models.PackingResult result)
	{
		return this.selectedResult == result;
	}
}
