using Binacle.Net.Api.UIModule.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Binacle.Net.Api.UIModule.Components.Features;

public class BinPackingBase : ComponentBase
{
	protected List<Item> Items { get; set; }
	protected Bin Bin { get; set; }

	[Inject]
	protected HttpClient HttpClient { get; set; }

	[Inject]
	protected IJSRuntime JsRuntime { get; set; }

	protected async Task BinChangedAsync()
	{
		await this.JsRuntime.InvokeVoidAsync("containerChanged", this.Bin);
	}

	protected void AddItem()
	{
		this.Items.Add(new Item(0, 0, 0, 1));
	}
	protected void RemoveItem(Item item)
	{
		this.Items.Remove(item);
	}
	protected void ClearAllItems()
	{
		this.Items.Clear();
	}

	protected override void OnInitialized()
	{
		//this.Items = new List<Item>();
		//this.Bin = new Bin(0, 0, 0);
		this.PopulateSampleData();
	}

	private void PopulateSampleData()
	{
		this.Bin = new Bin(60, 40, 10);
		this.Items = [
			new Item(2, 5, 10, 7),
			new Item(12, 15, 10, 3),
			new Item(10, 15, 15, 2)
		];
	}
}
