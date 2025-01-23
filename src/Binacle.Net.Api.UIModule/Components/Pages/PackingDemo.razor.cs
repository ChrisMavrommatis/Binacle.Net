using Binacle.Net.Api.UIModule.Services;
using Binacle.Net.Api.UIModule.ViewModels;
using Microsoft.AspNetCore.Components;

namespace Binacle.Net.Api.UIModule.Components.Pages;

public partial class PackingDemo : ComponentBase
{
	[Inject] internal ISampleDataService? SampleDataService { get; set; }

	protected Bin? SelectedBin { get; set; }
	protected List<Item>? SelectedItems { get; set; }

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

	protected override void OnParametersSet()
	{
		this.SelectedBin = this.Model.Bins.FirstOrDefault();
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

	internal void GetResults()
	{
		this.SelectedBin = this.Model.Bins.FirstOrDefault();
	}
}
